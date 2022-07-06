using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
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
                    var retryPolicy = Policy<HttpResponseMessage>
                        .Handle<RpcException>(r => r.Status.StatusCode != StatusCode.NotFound)
                        .Or<Exception>()
                        .WaitAndRetryAsync(3, retryAttempt => 
                            TimeSpan.FromSeconds(retryAttempt * 2));
                    
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                    
                    services.Configure<ScheduleOptions>(configuration.GetSection(nameof(ScheduleOptions)));
                    services.Configure<ForecastSourceOptions>(configuration.GetSection(nameof(ForecastSourceOptions)));
                    services.AddHostedService<WeatherDataCollectorHostedService>();
                    services.AddScoped<IGismeteoForecastSource, GismeteoForecastSource>();
                    services.AddScoped<IForecastEndpointGrpcClient, ForecastEndpointGrpcClient>();

                    var grpcOptions = configuration.GetSection(nameof(GrpcOptions)).Get<GrpcOptions>();
                    services
                        .AddGrpcClient<WeatherForecastSvc.Endpoint.Proto.WeatherForecastSvc.WeatherForecastSvcClient>(
                            opt => { opt.Address = new Uri(grpcOptions.Address); })
                        .AddPolicyHandler(retryPolicy);
                })
                .Build()
                .RunAsync();
        }
    }
}