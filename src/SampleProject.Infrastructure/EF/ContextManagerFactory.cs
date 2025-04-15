using Microsoft.EntityFrameworkCore;

namespace SampleProject.Infrastructure.EF
{
    public class ContextManagerFactory<TContext> : IContextManagerFactory<TContext> where TContext : DbContext
    {
        private readonly ContextManager<TContext> _contextManager;

        public ContextManagerFactory(ContextManager<TContext> contextManager)
        {
            _contextManager = contextManager;
        }

        public async Task<T> ExecuteThreadSafe<T>(Func<TContext, Task<T>> action)
        {
            var context = _contextManager.AcquireDbContext();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var result = await action(context);
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _contextManager.ReleaseDbContext();
            }
        }

        public T ExecuteThreadSafeSync<T>(Func<TContext, T> action)
        {
            var context = _contextManager.AcquireDbContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var result = action(context);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _contextManager.ReleaseDbContext();
            }
        }
    }
}
