using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WeatherForecastDataCollector
{
    public class WeatherDb
    {
        public void ListDatabases()
        {
            MongoClient client = new MongoClient("mongodb://test-mongo:27017");

            List<string> databases = client.ListDatabaseNames().ToList();

            foreach(string database in databases) {
                Console.WriteLine(database);
            }
        }

        public void Insert(WeatherPageSlice model)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            var slicesCollection = client.GetDatabase("weather_forecast_db").GetCollection<WeatherPageSlice>("slices");

            slicesCollection.InsertOne(model);
        }

        public void InsertCities(IEnumerable<City> cities)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            var citiesCollection = client.GetDatabase("weather_forecast_db").GetCollection<City>("cities");

            var filter = new BsonDocument();
            var existingCities = citiesCollection.FindSync(filter).ToList();
            var notExisting = cities.Except(existingCities, new CityComparer()).ToList();

            if (!notExisting.Any())
            {
                return;
            }

            citiesCollection.InsertMany(notExisting);
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