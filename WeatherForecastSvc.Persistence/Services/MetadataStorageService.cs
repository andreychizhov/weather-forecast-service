using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WeatherForecastSvc.Endpoint.Configuration;
using WeatherForecastSvc.Persistence.Models;

namespace WeatherForecastSvc.Persistence.Services
{
    public class MetadataStorageService : IMetadataStorageService
    {
        private readonly MongoDbSettings _settings;
        private readonly IMongoCollection<City> _cities;
        
        private const string CollectionName = "cities";

        public MetadataStorageService(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            
            var client = new MongoClient(settings.Value.ConnectionUri);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            
            _cities = database.GetCollection<City>(CollectionName);
        }
        
        public async Task<IReadOnlyCollection<City>> GetCities(CancellationToken token)
        {
            var filter = new BsonDocument();
            var result = await _cities.FindAsync(filter, cancellationToken: token);

            return result.ToList();
        }
    }
}