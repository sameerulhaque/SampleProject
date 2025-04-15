using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace SampleProject.Infrastructure.Consul
{
    public static class ConsulConfig
    {
        public static T GetSetting<T>(this IConfiguration config, string key)
        {
            var setting = config.GetSection("Settings").GetValue<T?>(key);
            if (setting == null) throw new Exception("Setting does not exist");
            return setting;
        }
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration.GetValue<string>("Consul:Host") ?? "";
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var registration = new AgentServiceRegistration()
            {
                ID = $"CMS",
                Name = "CMS",
                Address = "localhost",
                Port = 19322,
                Check = new AgentCheckRegistration()
                {
                    HTTP = "http://localhost:19322/health",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };
            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}
