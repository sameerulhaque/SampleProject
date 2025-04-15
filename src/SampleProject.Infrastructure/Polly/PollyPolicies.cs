using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using StackExchange.Redis;

namespace SampleProject.Infrastructure.Polly
{
    public static class RedisPolicies
    {
        public static AsyncRetryPolicy RetryPolicy { get; } =
            Policy
                .Handle<RedisException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (exception, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} due to {exception.Message}");
                    });

        public static AsyncFallbackPolicy<T> FallbackPolicy<T>(T fallbackValue) =>
            Policy<T>
                .Handle<RedisException>()
                .Or<SocketException>()
                .FallbackAsync(
                    fallbackValue: fallbackValue,
                    onFallbackAsync: (exception, context) =>
                    {
                        Console.WriteLine($"Fallback triggered due to {exception.Exception}");
                        return Task.CompletedTask;
                    });

        public static AsyncCircuitBreakerPolicy CircuitBreakerPolicy { get; } =
            Policy
                .Handle<RedisException>()
                .Or<SocketException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDelay) =>
                    {
                        Console.WriteLine($"Circuit breaker tripped due to {exception.Message}. Circuit will be open for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("Circuit breaker half-open. Allowing trial operations.");
                    });

        public static async Task<T> ExecuteWithPolicyAsync<T>(Func<Task<T>> operation, T fallbackValue)
        {
            return await FallbackPolicy(fallbackValue)
                .WrapAsync(RetryPolicy)
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(operation);
        }

    }

}

