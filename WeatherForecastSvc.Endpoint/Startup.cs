using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Serilog;
using WeatherForecastSvc.Endpoint.Configuration;
using WeatherForecastSvc.Endpoint.GprcServices;
using WeatherForecastSvc.Persistence.Services;

namespace WeatherForecastSvc.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddResponseCaching();

            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddGrpc();
            services.AddGrpcReflection();
            
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDB"));
            services.AddScoped<IForecastStorageService, ForecastStorageService>();
            services.AddScoped<IMetadataStorageService, MetadataStorageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseCors(policy => 
                policy.WithOrigins("https://localhost:7158", "http://localhost:5098")
                    .AllowAnyMethod()
                    .WithHeaders(HeaderNames.ContentType));
                
            app.UseSerilogRequestLogging();

            app.UseResponseCaching();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<WeatherForecastGrpcService>();
                endpoints.MapGrpcReflectionService();
            });
        }
    }
}
