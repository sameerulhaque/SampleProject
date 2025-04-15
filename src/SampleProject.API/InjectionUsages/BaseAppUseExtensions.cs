using BuildingBlocks.Application.Middlewares;
using Finbuckle.MultiTenant;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Prometheus;
using SampleProject.API.Middleware;
using SampleProject.Core.Exceptions;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Infrastructure.Consul;
using SampleProject.Infrastructure.RabbitMQ;
using SampleProject.Infrastructure.Services;
using SampleProject.Shared.Enums.Global;

namespace SampleProject.API.InjectionUsages;

public static class BaseAppUseExtensions
{
    public static IApplicationBuilder BaseAppUse(this IApplicationBuilder app)
    {
        app
            .UsingLocalization()
            .UsingCors()
            .UsingSwagger()
            .UsingHangfire()
            .UsingMetrics()
            .UsingMultiTenant()
            .UsingRouting()
            .UsingAuthorization()
            .UsingUserInfo()
            .UsingMiddlewares()
            .UsingEndpoints()
            .Extra()
            .UsingSerilog();

        return app;
    }

    public static IApplicationBuilder UsingMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<RateLimitMiddleware>();
        app.UseMiddleware<HttpResponseMiddleware>();
        app.UseMiddleware<IdempotentMiddleware>();

        return app;
    }

    public static IApplicationBuilder UsingCors(this IApplicationBuilder app)
    {
        app.UseCors("allowall");

        return app;
    }
    public static IApplicationBuilder Extra(this IApplicationBuilder app)
    {
        app.UseConsul();
        app.UseResponseCompression();
        app.UseStaticFiles();

        return app;
    } 
    public static IApplicationBuilder UsingMultiTenant(this IApplicationBuilder app)
    {
        app.UseMultiTenant();
        return app;
    }

    public static IApplicationBuilder UsingSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            //options.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
        });

        return app;
    }

    public static IApplicationBuilder UsingAuthorization(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static IApplicationBuilder UsingLocalization(this IApplicationBuilder app)
    {
        var supportedCultures = Enum
            .GetValues(typeof(Languages))
            .Cast<Languages>()
            .Select(x => x.ToString())
            .ToArray();

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        return app;
    }

    public static IApplicationBuilder UsingHangfire(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new BasicAuthAuthorizationFilter("admin", "admin") }
        });

        return app;
    }


    public static IApplicationBuilder UsingMetrics(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();

        return app;
    }
    public static IApplicationBuilder UsingUserInfo(this IApplicationBuilder app)
    {
        var userInfoService = app.ApplicationServices.GetRequiredService<UserInfoService>();
        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        UserInfoHelper.Initialize(userInfoService, httpContextAccessor);
        return app;
    }

    public static IApplicationBuilder UsingRouting(this IApplicationBuilder app)
    {
        app.UseRouting();
        return app;
    }

    public static IApplicationBuilder UsingEndpoints(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
            endpoints.MapMetrics("/metrics");
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Mobile API's service is running! version 1.0.1.63");
            });
            endpoints.MapPost("/Add", async (
             IRabbitMQService rabbitMQService,
             CancellationToken cancellationToken, [FromBody] string name) =>
            {
                await rabbitMQService.SendAddTestModelMessage(name, cancellationToken);
                return Results.Ok();
            });
        });
        return app;
    }

    public static IApplicationBuilder UsingSerilog(this IApplicationBuilder app)
    {
        //app.useseri

        return app;
    }
}
