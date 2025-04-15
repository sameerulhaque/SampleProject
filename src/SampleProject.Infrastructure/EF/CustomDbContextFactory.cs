using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleProject.Infrastructure.Tenant;

namespace SampleProject.Infrastructure.EF
{
    public class CustomDbContextFactory<TContext> : ICustomDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITenantService _tenantService;

        public CustomDbContextFactory(IServiceScopeFactory serviceScopeFactory, ITenantService tenantService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _tenantService = tenantService;
        }

        public TContext CreateDbContext()
        {
            var scope = _serviceScopeFactory.CreateScope();
            var tenant = _tenantService.GetTenant();
            var connectionString = tenant?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Tenant connection string is not available.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return (TContext)ActivatorUtilities.CreateInstance(scope.ServiceProvider, typeof(TContext), optionsBuilder.Options);
        }
    }
}
