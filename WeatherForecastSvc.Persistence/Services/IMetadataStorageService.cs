using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherForecastSvc.Persistence.Models;

namespace WeatherForecastSvc.Persistence.Services
{
    public interface IMetadataStorageService
    {
        Task<IReadOnlyCollection<City>> GetCities(CancellationToken token);
        Task AddCities(IEnumerable<City> cities, CancellationToken token);
    }
}