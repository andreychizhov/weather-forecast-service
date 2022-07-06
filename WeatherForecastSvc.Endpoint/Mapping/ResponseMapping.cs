using System.Collections.Generic;
using System.Linq;
using WeatherForecastSvc.Endpoint.Dto;
using WeatherForecastSvc.Persistence.Models;

namespace WeatherForecastSvc.Endpoint.Mapping
{
    public static class ResponseMapping
    {
        public static DateForecastDto Map(this WeatherForecast model)
        {
            return new DateForecastDto
            {
                Date = model.Date,
                Summary = model.Summary,
                MinTempC = model.MinTempC,
                MaxTempC = model.MaxTempC,
                WindMs = model.WindMs,
                PrecipitationMm = model.PrecipitationMm
            };
        }

        public static CityForecastDto Map(this CityForecastData model)
        {
            return new CityForecastDto
            {
                CityName = model.CityName,
                Forecasts = model.Forecasts.Select(Map).ToList()
            };
        }
    }
}