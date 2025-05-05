using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace SampleProject.API.InjectionUsages;


public static class DependencyInjection
{
    public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .RegisterDBContext(configuration)
            .RegisterAuthentication();
        //.RegisterValidator()
        //.RegisterSwagger();

        return services;
    }

    //private static IServiceCollection RegisterValidator(this IServiceCollection services)
    //{
    //    services.AddValidatorsFromAssemblyContaining(typeof(SampleModelMapper));

    //    return services;
    //}

    private static IServiceCollection RegisterDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VuexyContext>((serviceProvider, options) =>
        {
            var tenantInfo = serviceProvider.GetRequiredService<ITenantService>();
            options.UseSqlServer(tenantInfo.GetTenant()?.ConnectionString);
            options.AddInterceptors(new CommandInterceptor());
        });
        services.AddScoped(typeof(ICustomDbContextFactory<>), typeof(CustomDbContextFactory<>));
        services.AddScoped(typeof(ContextManager<>));
        services.AddScoped(typeof(IContextManagerFactory<>), typeof(ContextManagerFactory<>));

        services.AddScoped<IDapperDbConnectionFactory, DapperDbConnectionFactory>();
        return services;
    }

    private static IServiceCollection RegisterAuthentication(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("CanDeletePolicy", policy =>
            policy.RequireClaim("Permissions", "CanDelete"));

        return services;
    }

    //private static IServiceCollection RegisterSwagger(this IServiceCollection services)
    //{
    //    services.AddSwaggerExamplesFromAssemblyOf(typeof(SampleModelMapper));

    //    return services;
    //}
}
