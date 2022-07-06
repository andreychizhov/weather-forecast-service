using System;

namespace WeatherForecastSvc.Endpoint.Dto
{
    public class ConstraintsDto
    {
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}