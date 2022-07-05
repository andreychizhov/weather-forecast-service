using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using MoreLinq;
using WeatherForecastDataCollector.Helpers;
using WeatherForecastDataCollector.Options;

namespace WeatherForecastDataCollector
{
    public interface IGismeteoForecastSource
    {
        Task<IReadOnlyCollection<City>> FetchCities(CancellationToken token);
        Task<WeatherPageSlice> FetchWeatherDataSlice(IEnumerable<City> cities, CancellationToken token);
    }

    public class GismeteoForecastSource : IGismeteoForecastSource
    {
        private readonly IBrowsingContext _context;
        private readonly ForecastSourceOptions _options;

        private const int MaxParallelRequests = 5;
        
        public GismeteoForecastSource(IOptions<ForecastSourceOptions> options)
        {
            var config = Configuration.Default.WithDefaultLoader();
            _context = BrowsingContext.New(config);
            _options = options.Value;
        }
        public async Task<IReadOnlyCollection<City>> FetchCities(CancellationToken token)
        {
            var document = await _context.OpenAsync(_options.BaseUri, token);

            var cells = document.QuerySelectorAll(Selectors.CitiesSelector);
            var cities = cells.Children("div")
                .Where(t => t.ClassName == "list-item")
                .Select(t => new City{ Name = t.FirstElementChild.TextContent, Link = t.FirstElementChild.Attributes["href"].Value})
                .ToList();

            return cities;
        }

        public async Task<WeatherPageSlice> FetchWeatherDataSlice(IEnumerable<City> cities, CancellationToken token)
        {
            var chunks = cities.Batch(MaxParallelRequests);
            var cityForecasts = new List<CityForecastData>();
            
            foreach (var chunk in chunks)
            {
                var forecasts = await Task.WhenAll(from c in chunk select GetWeatherForCity(c, token));
                cityForecasts.AddRange(forecasts);
            }
            
            return new WeatherPageSlice
            {
                Timestamp = DateHelper.Now,
                CityForecasts = cityForecasts
            };
        }
        
        private async Task<CityForecastData> GetWeatherForCity(City city,
            CancellationToken token)
        {
            var cityUrl = new Uri(_options.BaseUri).Append(city.Link, "10-days").AbsoluteUri;
            var document = await _context.OpenAsync(cityUrl, token);

            var element = document.QuerySelector(Selectors.ForecastBaseSelector);
            
            var result = InitCollector();
            
            CollectDescription(element, result);
            CollectTemperature(element, result);
            CollectWindSpeed(element, result);
            CollectPrecipitation(element, result);
            
            var cityForecast = new CityForecastData
            {
                CityName = city.Name,
                Forecasts = result.Values.ToImmutableList()
            };
            return cityForecast;
        }

        static Dictionary<DateTime, WeatherForecast> InitCollector()
        {
            var result = new Dictionary<DateTime, WeatherForecast>();
            return result;
        }

        private void CollectTemperature(IElement cells, Dictionary<DateTime, WeatherForecast> collector)
        {
            var values = cells.QuerySelectorAll(Selectors.AvgTemperatureSelector)
                .Children("div")
                .Where(t => t.ClassName.Contains("value"));

            var today = DateHelper.NowDate;

            var date = today;
            foreach (var value in values)
            {
                var minTemp = value.Children
                    .First(x => x.ClassName == "mint")
                    .Children.FirstOrDefault(x => x.ClassName?.StartsWith("unit unit_temperature_c") ?? false)?
                    .TextContent;
                
                var maxTemp = value.Children
                    .First(x => x.ClassName == "maxt")
                    .Children.FirstOrDefault(x => x.ClassName?.StartsWith("unit unit_temperature_c") ?? false)?
                    .TextContent;

                SetFieldSafe(collector, date, wf =>
                {
                    wf.MinTempC = TryParseIntOrDefault(minTemp);
                    wf.MaxTempC = TryParseIntOrDefault(maxTemp);
                });

                date = date.AddDays(1);
            }
        }

        private void CollectDescription(IElement cells, Dictionary<DateTime, WeatherForecast> collector)
        {
            var values = cells.QuerySelectorAll(Selectors.SummarySelector)
                .Children("div")
                .Where(t => t.ClassName.Contains("row-item"));

            var today = DateHelper.NowDate;

            var date = today;
            foreach (var value in values)
            {
                var summary = value.Children
                    .FirstOrDefault(x => x.ClassName?.StartsWith("WeatherFront-icon tooltip") ?? false)?
                    .Attributes["data-text"].Value;
                
                SetFieldSafe(collector, date, wf => wf.Summary = summary);

                date = date.AddDays(1);
            }
        }

        private void CollectWindSpeed(IElement cells, Dictionary<DateTime, WeatherForecast> collector)
        {
            var values = cells.QuerySelectorAll(Selectors.WindSpeedSelector)
                .Children("div")
                .Where(t => t.ClassName.Contains("row-item"));

            var today = DateHelper.NowDate;

            var date = today;
            foreach (var value in values)
            {
                var wind = value.Children
                    .FirstOrDefault(x => x.ClassName?.StartsWith("wind-unit unit unit_wind_m_s") ?? false)?
                    .TextContent;

                SetFieldSafe(collector, date, wf => wf.WindMs = TryParseIntOrDefault(wind));

                date = date.AddDays(1);
            }
        }

        private void CollectPrecipitation(IElement cells, Dictionary<DateTime, WeatherForecast> collector)
        {
            var values = cells.QuerySelectorAll(Selectors.PrecipitationSelector)
                .Children("div")
                .Where(t => t.ClassName.Contains("row-item"));

            var today = DateHelper.NowDate;

            var date = today;
            foreach (var value in values)
            {
                var precipitation = value.Children
                    .FirstOrDefault(x => x.ClassName?.StartsWith("item-unit") ?? false)?
                    .TextContent;
                
                SetFieldSafe(collector, date, wf => wf.PrecipitationMm = TryParseFloatOrDefault(precipitation));

                date = date.AddDays(1);
            }
        }

        private int TryParseIntOrDefault(string tempString) => 
            int.TryParse((tempString ?? string.Empty).Trim(), out var temp) ? temp : int.MinValue;
        private double TryParseFloatOrDefault(string tempString) => double.TryParse(
            (tempString ?? string.Empty).Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture,
            out var temp) ? temp : float.NaN;

        private void SetFieldSafe(IDictionary<DateTime, WeatherForecast> map, DateTime date, Action<WeatherForecast> mutator)
        {
            if (map.ContainsKey(date))
            {
                mutator(map[date]);
            }
            else
            {
                var forecast = new WeatherForecast{ Date = date};
                mutator(forecast);
                map[date] = forecast;
            }
        }

    }
}