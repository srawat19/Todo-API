using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Runtime.CompilerServices;
using System.Threading.RateLimiting;


namespace Todo_API.Middlewares
{
    public static class RateLimitingMiddleware
    {
        public static IServiceCollection AddRateLimiter(this IServiceCollection services)
        {

            //services.AddRateLimiter(config =>
            //{

            //    config.AddTokenBucketLimiter(policyName: "token", options =>
            //{
            //    options.TokenLimit = 8;
            //    options.QueueLimit = 0;
            //    options.AutoReplenishment = true;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(2);

            //});
            //});

            services.AddRateLimiter(config =>
            {
                config.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                config.OnRejected = async (httpContext, cancellationToken) =>
                {
                    httpContext.HttpContext.Response.ContentType = "application/json";


                    await httpContext.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
                };


                config.AddPolicy("RoleBasedPolicy", httpContext =>
                {

                    var user = httpContext.User;
                    var isAuth = httpContext.User.Identity.IsAuthenticated;
                    var userRole = "unknown";

                    if (isAuth)
                    {
                        if (user.IsInRole("admin"))
                        {
                            userRole = "admin";
                        }
                        else
                            userRole = "user";
                    }

                    return RateLimitPartition.GetTokenBucketLimiter<string>(partitionKey: userRole, factory: userRole =>
                    {
                        return userRole switch
                        {
                            "admin" => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 20,
                                TokensPerPeriod = 10,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(20),
                                AutoReplenishment = true,
                                QueueLimit = 0

                            },

                            "user" => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 5,
                                TokensPerPeriod = 10,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                AutoReplenishment = true,
                                QueueLimit = 0
                            },
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 5,
                                TokensPerPeriod = 1,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                AutoReplenishment = true,
                                QueueLimit = 0,

                            }
                        };
                    });

                });

            });

            return services;
        }

    }
}
