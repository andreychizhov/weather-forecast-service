using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using WeatherForecastSvc.Endpoint.Proto;

namespace WeatherForecastDataCollector
{
    public interface IForecastEndpointGrpcClient
    {
        Task AddCities(IEnumerable<City> cities, CancellationToken token);
        Task AddForecastData(WeatherPageSlice slice, CancellationToken token);
    }

    public class ForecastEndpointGrpcClient : IForecastEndpointGrpcClient
    {
        private readonly WeatherForecastSvc.Endpoint.Proto.WeatherForecastSvc.WeatherForecastSvcClient _forecastSvcClient;

        public ForecastEndpointGrpcClient(WeatherForecastSvc.Endpoint.Proto.WeatherForecastSvc.WeatherForecastSvcClient forecastSvcClient)
        {
            _forecastSvcClient = forecastSvcClient;
        }

        public async Task AddCities(IEnumerable<City> cities, CancellationToken token)
        {
            var req = new AddCitiesRequest
            {
                Cities = { cities.Select(x => x.Map()) }
            };
            await _forecastSvcClient.AddCitiesV1Async(req, cancellationToken: token);
        }

        public async Task AddForecastData(WeatherPageSlice slice, CancellationToken token)
        {
            var req = new AddWebPageSliceRequest
            {
                Timestamp = slice.Timestamp.ToTimestamp(),
                CityForecasts = { slice.CityForecasts.Select(x => x.Map()) }
            };
            await _forecastSvcClient.AddWebPageSliceV1Async(req, cancellationToken: token);
        }
    }
}