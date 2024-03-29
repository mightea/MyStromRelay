using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyStromRelay.Models;

namespace MyStromRelay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var myStromRelayUrl = Environment.GetEnvironmentVariable("MYSTROM_RELAY_TARGET_URL");
            if (String.IsNullOrWhiteSpace(myStromRelayUrl))
            {
                throw new ArgumentException("The required environment variable MYSTROM_RELAY_TARGET_URL was not set");
            }

            services.AddControllers();
            if (!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HOMEASSISTANT_API_TOKEN")))
            {
                services.AddHttpClient(HomeassistantReporterModel.HTTP_CLIENT_NAME, c =>
                {
                    c.BaseAddress = new Uri(myStromRelayUrl);
                    c.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("HOMEASSISTANT_API_TOKEN"));
                    c.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
                services.AddSingleton<IReporterModel, HomeassistantReporterModel>();
            }

            if (!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MQTT_SERVER")))
            {
                services.AddSingleton<IReporterModel, MqttReporterModel>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
