using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrder order;
        private readonly IOrderService orderService;   
        public OrderController(IOrder _order, IOrderService _orderService)
        {
            order = _order;
            orderService = _orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await order.GetAllAsync();
            var (_, list) = OrderConversion.FromEntity(null, orders);
            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var result = await order.FindByIdAsync(id);
            var ( list,_) = OrderConversion.FromEntity(result, null);
            return Ok(list);
        }

        [HttpGet("client{clientId:int}")]
        public async Task<ActionResult<OrderDto>> GetClientOrders(int clientId)
        {
            var result = await order.GetOrderAsync(o=>o.ClientId ==clientId);
            return Ok(result);
        }


        [HttpGet("details{orderId:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderDetails(int orderId)
        {

            var result = await orderService.GetOrdersDetails(orderId);
            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {
            var getEntity = OrderConversion.ToEntity(orderDto);
            var response = await order.CreateAsync(getEntity);
            return response.Falg ? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
        {
            var getEntity = OrderConversion.ToEntity(orderDto);
            var response = await order.UpdateAsync(getEntity);
            return response.Falg ? Ok(response) : BadRequest(response);

        }

        [HttpDelete("order/{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var response = await order.DeleteAsync(id);
            return response.Falg ? Ok(response) : BadRequest(response);
        }

    }
}
