using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Logging;

namespace SampleProject.Infrastructure.Tenant
{
    public interface ITenantService
    {
        string GetTenantKey(string key);
        CustomTenantInfo GetTenant();
    }

    public class TenantService : ITenantService
    {
        private readonly IMultiTenantContextAccessor<CustomTenantInfo> _multiTenantContextAccessor;
        private readonly ILogger<TenantService> _logger;

        public TenantService(IMultiTenantContextAccessor<CustomTenantInfo> multiTenantContextAccessor, ILogger<TenantService> logger)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _logger = logger;
        }

        public string GetTenantKey(string key)
        {
            var tenantInfo = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
            _logger.LogInformation($"Resolved Tenant: {tenantInfo?.Identifier}");
            return $"{tenantInfo?.Identifier ?? ""}:{key}";
        }

        public CustomTenantInfo GetTenant()
        {
            return _multiTenantContextAccessor.MultiTenantContext?.TenantInfo ?? new CustomTenantInfo();
        }
    }
}
