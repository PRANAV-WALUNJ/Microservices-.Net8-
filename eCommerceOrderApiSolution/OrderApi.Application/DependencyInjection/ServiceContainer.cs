﻿using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddserviceColleaction(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IOrderService,OrderService>(options =>
            {
                options.BaseAddress = new Uri(configuration["ApiGetway:BaseAddress"]!);
                options.Timeout =TimeSpan.FromSeconds(1);
            });

            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(3),

                OnRetry = args =>
                {
                    string message =$"OnRetry, Attempt: {args.AttemptNumber } Outcome { args.Outcome}";
                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);
                    return ValueTask.CompletedTask;

                }
            };

            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });
            return services;
        }
    }
}
