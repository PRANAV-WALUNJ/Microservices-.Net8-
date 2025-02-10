using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(IServiceCollection services, IConfiguration configuration, string filename) where TContext : DbContext
        {
            services.AddDbContext<TContext>(option =>

                option.UseSqlServer(configuration.GetConnectionString("eCommerceConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));

            //configure serilog

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{filename}-text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{TimeStamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedPolices(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            //app.UseMiddleware<ListenToOnlyApiGetway>();
            return app;
        }
    }
}
