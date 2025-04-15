using Microsoft.EntityFrameworkCore;

namespace SampleProject.Infrastructure.EF
{
    public interface ICustomDbContextFactory<TContext> where TContext : DbContext
    {
        TContext CreateDbContext();
    }
}
