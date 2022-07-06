using System.Collections.Generic;

namespace WeatherForecastSvc.Persistence.Models
{
    public class CityForecastData
    {
        public string CityName { get; set; }

        public IReadOnlyCollection<WeatherForecast> Forecasts { get; set; }
    }
}