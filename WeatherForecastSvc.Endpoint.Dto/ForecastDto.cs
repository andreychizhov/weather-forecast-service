using System.Collections.Generic;

namespace WeatherForecastSvc.Endpoint.Dto
{
    public class ForecastDto
    {
        public List<CityForecastDto> CityForecasts { get; set; }
        public ConstraintsDto Constraints { get; set; }
    }
}