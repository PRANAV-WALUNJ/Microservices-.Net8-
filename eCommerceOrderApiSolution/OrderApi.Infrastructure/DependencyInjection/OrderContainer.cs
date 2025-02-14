using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class OrderContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection service,IConfiguration configuration)
        {
            SharedServiceContainer.AddSharedServices<OrderDbContext>(service, configuration, configuration["MySerilog:FileName"]!);

            service.AddScoped<IOrder, OrderRepository>();
            return service;

        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            SharedServiceContainer.UseSharedPolices(app);
            return app;

        }
    }
}
