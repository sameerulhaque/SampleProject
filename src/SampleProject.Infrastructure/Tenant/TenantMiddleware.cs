using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SampleProject.Infrastructure.Tenant
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TenantMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var tenantId = httpContext.Request.Headers["X-Tenant-ID"].ToString();

            if (string.IsNullOrEmpty(tenantId))
            {
                //httpContext.Response.StatusCode = 400;
                //await httpContext.Response.WriteAsync("Tenant ID is missing in the request header.");
                //return;
                tenantId = "1";
            }

            var tenantsConfig = _configuration.GetSection("Finbuckle:MultiTenant:Stores:ConfigurationStore:Tenants").Get<List<CustomTenantInfo>>();
            var tenant = tenantsConfig?.FirstOrDefault(t => t.Id == tenantId);

            if (tenant == null)
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync("Tenant not found.");
                return;
            }

            var tenantInfo = new CustomTenantInfo
            {
                Id = tenant.Id,
                Identifier = tenant.Identifier,
                ConnectionString = tenant.ConnectionString,
                Name = tenant.Name
            };

            await _next(httpContext);
        }
    }

}
