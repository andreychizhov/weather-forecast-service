using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WeatherForecastSvc.Endpoint.Configuration;
using WeatherForecastSvc.Persistence.Models;

namespace WeatherForecastSvc.Persistence.Services
{
    public class ForecastStorageService : IForecastStorageService
    {
        private readonly MongoDbSettings _settings;
        private readonly IMongoCollection<WeatherPageSlice> _slices;
        private const string CollectionName = "slices";

        public ForecastStorageService(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            
            var client = new MongoClient(settings.Value.ConnectionUri);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            
            _slices = database.GetCollection<WeatherPageSlice>(CollectionName);
        }

        public async Task<WeatherForecast> GetForecast(string cityName, DateTime date, CancellationToken token)
        {
            var filter = new BsonDocument();
            var options = new FindOptions
            {
                MaxTime = TimeSpan.FromMilliseconds(_settings.OperationTimeout)
            };

            var slice = await _slices
                .Find(filter, options)
                .SortByDescending(s => s.Timestamp)
                .Limit(1)
                .FirstAsync(token);

            var cityForecast = slice.CityForecasts.FirstOrDefault(x =>
                x.CityName.Equals(cityName, StringComparison.InvariantCultureIgnoreCase));

            var forecast = cityForecast?.Forecasts.FirstOrDefault(x => x.Date == date.Date.Date);

            return forecast;
        }
    }
}