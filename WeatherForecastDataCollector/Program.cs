using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WeatherForecastDataCollector.Options;

namespace WeatherForecastDataCollector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            
            await Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                .ConfigureServices(services =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                    
                    services.Configure<ScheduleOptions>(configuration.GetSection("ScheduleOptions"));
                    services.Configure<ForecastSourceOptions>(configuration.GetSection("ForecastSourceOptions"));
                    services.AddHostedService<WeatherDataCollectorHostedService>();
                    services.AddScoped<IGismeteoForecastSource, GismeteoForecastSource>();
                })
                .Build()
                .RunAsync();
        }
    }
}