using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherForecastDataCollector
{
    public class City
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
    
    public class WeatherPageSlice
    {
        public DateTime Timestamp { get; set; }

        [BsonElement("items")]
        [JsonPropertyName("items")]
        public IReadOnlyList<CityForecastData> CityForecasts { get; set; }
    }
        
    public class CityForecastData
    {
        public string CityName { get; set; }

        public IReadOnlyList<WeatherForecast> Forecasts { get; set; }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public int MinTempC { get; set; }
        public int MaxTempC { get; set; }
        public int WindMs { get; set; }
        public double PrecipitationMm { get; set; }
    }
}