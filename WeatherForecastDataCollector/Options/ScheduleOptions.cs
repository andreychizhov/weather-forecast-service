namespace WeatherForecastDataCollector.Options
{
    public class ScheduleOptions
    {
        public string Cron { get; set; }
        public int ErrorDelayTimeSec { get; set; }
    }
}