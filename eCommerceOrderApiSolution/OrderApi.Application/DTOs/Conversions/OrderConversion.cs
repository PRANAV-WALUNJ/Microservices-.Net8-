using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDto order) => new Order
        {
            Id = order.ID,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderDate = order.OrderDate,
            PurchaseQuantity = order.PurchaseQuantity
        };

        public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
           if(order is not null || orders is null)
            {
                var singleOrder = new OrderDto(
                    order!.Id,
                    order.ClientId,
                    order.ProductId,
                    order.ProductId,
                    order.OrderDate
                );

                return (singleOrder,null);
           }


            if (order is null || orders is not null)
            {
                var orderList = orders!.Select(o => new OrderDto(
                    o.Id,
                    o.ClientId,
                    o.ProductId,
                    o.PurchaseQuantity,
                    o.OrderDate)
                );

                return (null, orderList);
            }
            return (null, null);
        }
    }
}
