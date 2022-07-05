using System;

namespace WeatherForecastDataCollector.Helpers
{
    public static class DateHelper
    {
        public static DateTime NowDate => Now.Date;
        public static DateTime Now => DateTime.UtcNow;
    }
}