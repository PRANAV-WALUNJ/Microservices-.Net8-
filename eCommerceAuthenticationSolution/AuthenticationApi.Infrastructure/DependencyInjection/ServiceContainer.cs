using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrasturctureService(this IServiceCollection services,IConfiguration configuration)
        {
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, configuration,"");
            return services;
        }
    }
}
