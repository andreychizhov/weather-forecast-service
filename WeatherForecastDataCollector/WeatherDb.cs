using System;
using System.Collections.Generic;
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
    }
}