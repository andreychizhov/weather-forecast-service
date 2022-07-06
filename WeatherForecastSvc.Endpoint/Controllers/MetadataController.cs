using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastSvc.Endpoint.Dto;
using WeatherForecastSvc.Persistence.Services;

namespace WeatherForecastSvc.Endpoint.Controllers
{
    [ApiController]
    [Route("metadata")]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataStorageService _metadataService;

        public MetadataController(IMetadataStorageService metadataService)
        {
            _metadataService = metadataService;
        }

        [HttpGet]
        [Route("cities")]
        [ResponseCache(Duration = 600)]
        [ProducesResponseType(typeof(CityDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCities(CancellationToken token)
        {
            var cities = await _metadataService.GetCities(token);
            return Ok(cities.Select(x => new CityDto{ Name = x.Name }));
        }
    }
}