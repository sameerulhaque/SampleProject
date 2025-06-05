using Azure;
using BuildingBlocks.Application.Middlewares;
using Finbuckle.MultiTenant;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SampleProject.API.Swagger;
using SampleProject.Application.Interfaces;
using SampleProject.Application.Services;
using SampleProject.Core.Interfaces;
using SampleProject.Core.Services;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Consul;
using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Infrastructure.HealthMonitoring;
using SampleProject.Infrastructure.Mappings;
using SampleProject.Infrastructure.RabbitMQ;
using SampleProject.Infrastructure.Services;
using SampleProject.Infrastructure.Tenant;
using Serilog;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

namespace SampleProject.API.InjectionUsages;

public static class BaseDependencyInjection
{
    public static IServiceCollection BaseRegister(this IServiceCollection services,
        IConfiguration configuration, IHostBuilder hostBuilder)
    {
        services
            .RegisterControllers()
            .RegisterAPIVersioning()
            .RegisterLog(configuration, hostBuilder)
            .RegisterTenant()
            .Extra(configuration)
            .RegisterMemoryCache()
            //.RegisterRedis(configuration)
            .RegisterCache()
            .RegisterAuthentication(configuration)
            .RegisterCurrentUser()
            .RegisterSwagger()
            .RegisterCors()
            .RegisterHealthcheck()
            .RegisterLocalization()
            .RegisterFeatureManagement()
            .RegisterHangfire()
            .RegisterMappingProfile()
            .RegisterGenericService()
            .RegisterRabitMQ(configuration);

        return services;
    }

    private static IServiceCollection RegisterControllers(this IServiceCollection services)
    {

        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection RegisterAPIVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-API-Version"));
            });
        return services;
    }

    public static IServiceCollection RegisterLog(this IServiceCollection services,
        IConfiguration configuration, IHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
       .ReadFrom.Configuration(configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console()
       .CreateLogger();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog(dispose: true);
        });
        return services;
    }

    public static IServiceCollection RegisterMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();

        return services;
    }
    public static IServiceCollection RegisterCache(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<DapperQueryBuilder>();

        return services;
    }
    public static IServiceCollection Extra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ProblemDetailsFactory, ValidaitonExceptionService>();
        services.AddConsulConfig(configuration);
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/json", "application/xml" });
        }); return services;
    }

    public static IServiceCollection RegisterRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var cfg = configuration.GetSection("Redis").GetValue<string>("Host") + ":" + configuration.GetSection("Redis").GetValue<string>("Port");
            return ConnectionMultiplexer.Connect(cfg!);
        });
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        return services;
    }

    private static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Request is forbidden"
                        });

                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Key"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "AlternativeKey"))
                };
            });

        return services;
    }

    private static IServiceCollection RegisterCurrentUser(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizePolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
        services.AddSingleton<UserInfoService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        return services;
    }
    private static IServiceCollection RegisterTenant(this IServiceCollection services)
    {
        services.AddScoped<CustomTenantInfo>();
        services.AddMultiTenant<CustomTenantInfo>()
        .WithConfigurationStore()
        .WithHeaderStrategy("X-Tenant-ID");

        services.AddSingleton<ITenantService, TenantService>();
        return services;
    }

    private static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "sample", Version = "v1", Contact = new OpenApiContact() { Email = "sameerulhaque1997@gmail.com", Name = "Argaam IT", Url = new Uri("https://bivts.com") } });
            c.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme()
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "jwt_auth",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new string[] { }},
                });
            c.OperationFilter<AddTenantIdHeaderParameter>();
            c.OperationFilter<AddPaginationAndSearchHeaders>();
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    var controllerName = controllerActionDescriptor.ControllerName;
                    var excludedControllers = new string[] {  };
                    return !excludedControllers.Contains(controllerName);
                }
                return true;
            }
            );
        });

        return services;
    }

    private static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("allowall", policy =>
            {
                policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection RegisterHealthcheck(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<ApiHealthCheck>("Macro Indicator MicroService Health Check", tags: new string[] { "Indicator health api" });

        return services;
    }

    public static IServiceCollection RegisterLocalization(this IServiceCollection services)
    {
        services.AddLocalization();

        return services;
    }

    public static IServiceCollection RegisterFeatureManagement(this IServiceCollection services)
    {
        services.AddFeatureManagement();

        return services;
    }
    public static IServiceCollection RegisterRabitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRabbitMQService, RabbitMQService>();
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));
        return services;
    }
    public static IServiceCollection RegisterMappingProfile(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
    public static IServiceCollection RegisterGenericService(this IServiceCollection services)
    {
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IKYCService, KYCService>();
        return services;
    }

    public static IServiceCollection RegisterHangfire(this IServiceCollection services)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseDefaultTypeSerializer()
            .UseMemoryStorage());

        services.AddHangfireServer();

        return services;
    }
}
