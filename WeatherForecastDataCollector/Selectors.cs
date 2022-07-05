namespace WeatherForecastDataCollector
{
    public static class Selectors
    {
        public const string CitiesSelector = "body > section > div.content-column.column1 > section:nth-child(2) > div > div.cities-popular > div.list";
        public const string ForecastBaseSelector = "body > section.content.wrap > div.content-column.column1 > section:nth-child(2) > div > div > div > div";
        public const string AvgTemperatureSelector = "div.widget-row-chart.widget-row-chart-temperature > div > div";
        public const string SummarySelector = "div.widget-row.widget-row-icon";
        public const string WindSpeedSelector = "div.widget-row.widget-row-wind-gust.row-with-caption";
        public const string PrecipitationSelector = @"div.widget-row.widget-row-precipitation-bars.row-with-caption";
    }
}