using System;

namespace WeatherForecastSvc.Persistence.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public int MinTempC { get; set; }
        public int MaxTempC { get; set; }
        public int WindMs { get; set; }
        public double PrecipitationMm { get; set; }
    }
}