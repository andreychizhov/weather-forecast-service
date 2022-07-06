using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherForecastSvc.Persistence.Models
{
    public class City
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
        public string Link { get; set; }
    }
}