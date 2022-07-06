using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherForecastSvc.Endpoint.Dto;
using WeatherForecastSvc.Endpoint.Mapping;
using WeatherForecastSvc.Persistence.Services;

namespace WeatherForecastSvc.Endpoint.Controllers
{
    [ApiController]
    [Route("forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IForecastStorageService _forecastService;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IForecastStorageService forecastService, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _forecastService = forecastService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DateForecastDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(
            [FromQuery] string cityName,
            [FromQuery] DateTime date,
            CancellationToken token)
        {
            var forecast = await _forecastService.GetForecast(cityName, date, token);
            if (forecast == null)
                return NotFound("Unable to find data for specified input");

            return Ok(forecast.Map());
        }
        
        [HttpGet]
        [Route("cached")]
        // Cities count is relatively small, so we can cache all combinations
        [ResponseCache(VaryByQueryKeys = new [] {"cityName", "date"}, Duration = 600)]
        [ProducesResponseType(typeof(DateForecastDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCached(
            [FromQuery] string cityName,
            [FromQuery] DateTime date,
            CancellationToken token)
        {
            var forecast = await _forecastService.GetForecast(cityName, date, token);
            if (forecast == null)
                return NotFound("Unable to find data for specified input");

            return Ok(forecast.Map());
        }
        
        [HttpGet]
        [Route("full")]
        [ProducesResponseType(typeof(ForecastDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var forecast = await _forecastService.GetLatestForecasts(token);
            if (forecast == null)
                return NotFound("Unable to find data for specified input");

            return Ok(new ForecastDto
            {
                CityForecasts = forecast.Select(ResponseMapping.Map).ToList()
            });
        }
    }
}
