using System;
using System.Collections.Generic;
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

        public async Task AddCities(IEnumerable<City> cities, CancellationToken token)
        {
            var filter = new BsonDocument();
            var options = new FindOptions<City, City>
            {
                MaxTime = TimeSpan.FromMilliseconds(_settings.OperationTimeout)
            };
            
            var existingCities = _cities.FindSync(filter, options).ToList();
            var toInsert = cities.Except(existingCities, new CityComparer()).ToList();

            if (toInsert.Any())
            {
                await _cities.InsertManyAsync(toInsert, cancellationToken: token);
            }

            var toDelete = existingCities.Except(cities, new CityComparer()).ToList();

            if (toDelete.Any())
            {
                foreach (var city in toDelete)
                {
                    var deleteFilter = Builders<City>.Filter.Eq("Name", city.Name);
                    await _cities.DeleteOneAsync(deleteFilter, token);
                }
            }
        }
        
        class CityComparer : IEqualityComparer<City>
        {
            public bool Equals(City x, City y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(City obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }
    }
}