using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherForecastSvc.Persistence.Models
{
    public class WeatherPageSlice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public DateTime Timestamp { get; set; }

        [BsonElement("items")]
        [JsonPropertyName("items")]
        public IReadOnlyList<CityForecastData> CityForecasts { get; set; }
    }
}