using Microsoft.Data.SqlClient;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;

namespace SampleProject.Infrastructure.Polly
{
    public static class EfCorePolices
    {
        public static AsyncRetryPolicy RetryPolicy { get; } =
            Policy
                .Handle<SqlException>()
                .Or<TimeoutException>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                    onRetry: (exception, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} due to: {exception.Message}");
                    });

        public static AsyncCircuitBreakerPolicy CircuitBreakerPolicy { get; } =
            Policy
                .Handle<SqlException>()
                .Or<TimeoutException>()
                .Or<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDelay) =>
                    {
                        Console.WriteLine($"Circuit breaker tripped due to: {exception.Message}. Circuit will be open for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("Circuit breaker half-open. Allowing trial operations.");
                    });

        public static AsyncFallbackPolicy<TEntity> FallbackPolicy<TEntity>(TEntity fallbackValue) =>
            Policy<TEntity>
                .Handle<SqlException>()
                .Or<TimeoutException>()
                .Or<Exception>()
                .FallbackAsync(
                    fallbackValue: fallbackValue,
                    onFallbackAsync: (exception, context) =>
                    {
                        Console.WriteLine($"Fallback triggered due to: {exception.Exception.Message}");
                        return Task.CompletedTask;
                    });

        public static async Task<T> ExecuteWithPollyAsync<T>(
        Func<Task<T>> operation,
        T fallbackValue = default,
        CancellationToken cancellationToken = default)
        {
            return await EfCorePolices.RetryPolicy
                .WrapAsync(EfCorePolices.CircuitBreakerPolicy)
                .WrapAsync(EfCorePolices.FallbackPolicy(fallbackValue))
                .ExecuteAsync(operation);
        }
    }

}

