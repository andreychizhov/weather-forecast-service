using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherForecastSvc.Persistence.Models;

namespace WeatherForecastSvc.Persistence.Services
{
    public interface IForecastStorageService
    {
        Task<WeatherForecast> GetForecast(string cityName, DateTime date, CancellationToken token);
        Task<IReadOnlyList<CityForecastData>> GetLatestForecasts(CancellationToken token);
        Task AddForecasts(WeatherPageSlice forecastData, CancellationToken token);
    }
}