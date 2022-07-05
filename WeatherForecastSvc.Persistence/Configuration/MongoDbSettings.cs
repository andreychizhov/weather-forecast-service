namespace WeatherForecastSvc.Endpoint.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionUri { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public int OperationTimeout { get; set; }
    }
}