using System.Collections.Generic;

namespace WeatherForecastSvc.Endpoint.Dto
{
    public class CityForecastDto
    {
        public string CityName { get; set; }
        public List<DateForecastDto> Forecasts { get; set; }
    }
}