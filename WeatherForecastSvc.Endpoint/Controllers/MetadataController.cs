using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeatherForecastSvc.Endpoint.Controllers
{
    [ApiController]
    [Route("metadata")]
    public class MetadataController : ControllerBase
    {
        [HttpGet]
        [Route("cities")]
        [ProducesResponseType(typeof(CityDto[]), StatusCodes.Status200OK)]
        public IActionResult GetCities()
        {
            return Ok(new[] {"Барнаул", "Новосибирск", "Екатеринбург"}.Select(x => new CityDto{ Name = x }));
        }
    }

    class CityDto
    {
        public string Name { get; set; }
    }    
}