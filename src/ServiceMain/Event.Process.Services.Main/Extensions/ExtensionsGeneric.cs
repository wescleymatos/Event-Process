using Polly;
using Polly.Retry;

namespace Event.Process.Services.Main.Extensions;

public static class ExtensionsGeneric
{
    public static TimeSpan AsMessageRateToSleepTimeSpan(this int messagesPerSecond)
        {
            if (messagesPerSecond < 1)
                throw new ArgumentOutOfRangeException(nameof(messagesPerSecond));

            var sleepTimer = 1000 / messagesPerSecond;

            return TimeSpan.FromMilliseconds(sleepTimer);
        }

        public static void Wait(this TimeSpan time)        
        {
            System.Threading.Thread.Sleep(time);
        }

        public static IServiceCollection AddTransientWithRetry<TService, TKnowException>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
            where TKnowException : Exception
            where TService : class
        {
            return services.AddTransient(sp =>
            {
                TService returnValue = default;
                RetryPolicy policy = BuildPolicy<TKnowException>();

                policy.Execute(() => returnValue = implementationFactory(sp));

                return returnValue;
            });
        }

        public static IServiceCollection AddSingletonWithRetry<TService, TKnowException>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
            where TKnowException : Exception
            where TService : class
        {
            return services.AddSingleton(sp =>
            {
                TService returnValue = default;

                BuildPolicy<TKnowException>().Execute(() => { returnValue = implementationFactory(sp); });

                return returnValue;
            });
        }

        public static IServiceCollection AddScopedWithRetry<TService, TKnowException>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
           where TKnowException : Exception
           where TService : class
        {
            return services.AddScoped(sp =>
            {
                TService returnValue = default;

                BuildPolicy<TKnowException>().Execute(() => returnValue = implementationFactory(sp));

                return returnValue;
            });
        }


        private static RetryPolicy BuildPolicy<TKnowException>(int retryCount = 5) where TKnowException : Exception
        {
            return Policy
                .Handle<TKnowException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
        }
}
