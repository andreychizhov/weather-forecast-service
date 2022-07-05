using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherForecastSvc.Endpoint.Dto;
using WeatherForecastSvc.Persistence.Models;
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
        [ProducesResponseType(typeof(ForecastDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(
            [FromQuery] string cityName,
            [FromQuery] DateTime date,
            CancellationToken token)
        {
            var forecast = await _forecastService.GetForecast(cityName, date, token);
            if (forecast == null)
                return NotFound("Unable to find data for specified input");

            return Ok(forecast);
        }
    }
}
