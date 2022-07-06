using System.Linq;
using Google.Protobuf.WellKnownTypes;
using WeatherForecastSvc.Endpoint.Proto;

namespace WeatherForecastDataCollector
{
    public static class Mapping
    {
        public static WeatherForecastSvc.Endpoint.Proto.City Map(this City x) => 
            new WeatherForecastSvc.Endpoint.Proto.City {Name = x.Name, Link = x.Link};
        
        public static CityForecast Map(this CityForecastData data) =>
            new CityForecast {CityName = data.CityName, Forecasts = {data.Forecasts.Select(Map)}};

        private static WeatherForecastSvc.Endpoint.Proto.WeatherForecast Map(WeatherForecast forecast) =>
            new WeatherForecastSvc.Endpoint.Proto.WeatherForecast
            {
                Date = forecast.Date.ToTimestamp(),
                Summary = forecast.Summary,
                MinTempC = forecast.MinTempC,
                MaxTempC = forecast.MaxTempC,
                WindMs = forecast.WindMs,
                PrecipitationMm = forecast.PrecipitationMm
            };
    }
}