using System;

namespace WeatherForecastSvc.Endpoint.Dto
{
    public class ForecastDto
    {
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public int MinTempC { get; set; }
        public int MaxTempC { get; set; }
        public int WindMs { get; set; }
        public double PrecipitationMm { get; set; }
    }
}