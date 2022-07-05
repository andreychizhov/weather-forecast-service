using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherForecastDataCollector.Helpers;
using WeatherForecastDataCollector.Options;

namespace WeatherForecastDataCollector
{
    public class WeatherDataCollectorHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ScheduleOptions _options;
        private readonly ILogger<WeatherDataCollectorHostedService> _logger;

        public WeatherDataCollectorHostedService(IServiceProvider serviceProvider, IOptions<ScheduleOptions> options, ILogger<WeatherDataCollectorHostedService> logger)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sw = new Stopwatch();
                try
                {
                    var expression = CronExpression.Parse(_options.Cron);
                    var nextUtc = expression.GetNextOccurrence(DateHelper.Now);
                    
                    if (nextUtc == null) return;
                    _logger.LogInformation("Next start at {0:g}", nextUtc);

                    await Task.Delay(nextUtc.Value - DateHelper.Now, stoppingToken);
                    
                    _logger.LogInformation("Starting");

                    sw.Start();
                    await ExecuteInternalAsync(stoppingToken);
                    sw.Stop();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Execution canceled");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Processing error");
                    await Task.Delay(TimeSpan.FromSeconds(_options.ErrorDelayTimeSec), stoppingToken);
                }
                
                _logger.LogInformation("Execution succeed at {0}", DateHelper.Now);
                _logger.LogInformation("Execution time: {0:g}", sw.Elapsed);
            }
        }

        private async Task ExecuteInternalAsync(CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            
            var forecastSource = scope.ServiceProvider.GetRequiredService<IGismeteoForecastSource>();

            var links = await forecastSource.FetchCities(token);

            var root = await forecastSource.FetchWeatherDataSlice(links, token); 
            
            var db = new WeatherDb();
            db.Insert(root);
        }
    }
}