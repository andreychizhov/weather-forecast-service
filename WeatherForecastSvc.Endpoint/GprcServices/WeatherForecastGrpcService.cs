using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using WeatherForecastSvc.Endpoint.Mapping;
using WeatherForecastSvc.Endpoint.Proto;
using WeatherForecastSvc.Persistence.Models;
using WeatherForecastSvc.Persistence.Services;

namespace WeatherForecastSvc.Endpoint.GprcServices
{
    public class WeatherForecastGrpcService : Proto.WeatherForecastSvc.WeatherForecastSvcBase
    {
        private readonly IForecastStorageService _forecastService;
        private readonly IMetadataStorageService _metadataService;

        public WeatherForecastGrpcService(IForecastStorageService forecastService, IMetadataStorageService metadataService)
        {
            _forecastService = forecastService;
            _metadataService = metadataService;
        }

        public override async Task<AddWebPageSliceResponse> AddWebPageSliceV1(AddWebPageSliceRequest request, ServerCallContext context)
        {
            var data = new WeatherPageSlice
            {
                Timestamp = request.Timestamp.ToDateTime(),
                CityForecasts = request.CityForecasts.Select(x => x.Map()).ToList()
            };

            await _forecastService.AddForecasts(data, context.CancellationToken);
            
            return new AddWebPageSliceResponse();
        }
        

        public override async Task<AddCitiesResponse> AddCitiesV1(AddCitiesRequest request, ServerCallContext context)
        {
            var data = request.Cities.Select(x => x.Map());
            await _metadataService.AddCities(data, context.CancellationToken);

            return new AddCitiesResponse();
        }
    }
}