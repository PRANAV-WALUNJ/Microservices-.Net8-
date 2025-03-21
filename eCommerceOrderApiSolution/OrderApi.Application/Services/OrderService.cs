using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface ,HttpClient httpClient,ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        public async Task<ProductDto> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/product/{productId}");
                if (!getProduct.IsSuccessStatusCode)
                return null;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
            return product!;
        }

        public async Task<AppUserDto> GetUser(int userID)
        {
            var getUser = await httpClient.GetAsync($"/api/authentication/{userID}");
            if (!getUser.IsSuccessStatusCode)
                return null;

            var product = await getUser.Content.ReadFromJsonAsync<AppUserDto>();
            return product!;
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
        {

            var orders= await orderInterface.GetOrderAsync(o=>o.ClientId == clientId);
            if (!orders.Any())
                return null;

            var (_,_orders) = OrderConversion.FromEntity(null, orders);
            return _orders;
        }

        public async Task<OrderDetailsDto> GetOrdersDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);    
            if (order == null)
                return null;

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");
            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));
            var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDto(
                order.Id,
                productDto.Id,
                appUserDto.Id,
                appUserDto.Name,
                appUserDto.Email,
                appUserDto.Address,
                appUserDto.TelephoneNumber,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.Price,
                productDto.Quantity * order.PurchaseQuantity,
                order.OrderDate);
        }
    }
}
