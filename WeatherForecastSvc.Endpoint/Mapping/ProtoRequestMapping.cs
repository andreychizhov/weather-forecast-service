using System.Linq;
using WeatherForecastSvc.Endpoint.Proto;
using WeatherForecastSvc.Persistence.Models;
using City = WeatherForecastSvc.Persistence.Models.City;
using WeatherForecast = WeatherForecastSvc.Persistence.Models.WeatherForecast;

namespace WeatherForecastSvc.Endpoint.Mapping
{
    public static class ProtoRequestMapping
    {
        public static CityForecastData Map(this CityForecast cityForecast)
        {
            return new CityForecastData
            {
                CityName = cityForecast.CityName,
                Forecasts = cityForecast.Forecasts.Select(Map).ToList()
            };
        }

        private static WeatherForecast Map(Proto.WeatherForecast proto)
        {
            return new WeatherForecast();
        }

        public static City Map(this Proto.City proto)
        {
            return new City
            {
                Name = proto.Name,
                Link = proto.Link
            };
        }
    }
}