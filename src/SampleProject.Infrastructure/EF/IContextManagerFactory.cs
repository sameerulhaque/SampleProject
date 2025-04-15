using Microsoft.EntityFrameworkCore;

namespace SampleProject.Infrastructure.EF
{
    public interface IContextManagerFactory<TContext> where TContext : DbContext
    {
        Task<T> ExecuteThreadSafe<T>(Func<TContext, Task<T>> action);
        T ExecuteThreadSafeSync<T>(Func<TContext, T> action);
    }
}
