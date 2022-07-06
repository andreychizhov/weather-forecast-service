using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
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
                    services.AddScoped<IForecastEndpointGrpcClient, ForecastEndpointGrpcClient>();

                    AppContext.SetSwitch(
                        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    
                    services
                        .AddGrpcClient<WeatherForecastSvc.Endpoint.Proto.WeatherForecastSvc.WeatherForecastSvcClient>(
                            opt =>
                            {
                                opt.Address = new Uri("http://localhost:5126");
                                opt.ChannelOptionsActions.Add(channelOptions =>
                                    channelOptions.Credentials = ChannelCredentials.Insecure);
                            });
                })
                .Build()
                .RunAsync();
        }
    }
}