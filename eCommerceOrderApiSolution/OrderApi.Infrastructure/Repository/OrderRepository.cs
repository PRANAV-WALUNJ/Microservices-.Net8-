using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repository
{
    public class OrderRepository(OrderDbContext orderDbContext) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = orderDbContext.Orders.Add(entity).Entity;
                await orderDbContext.SaveChangesAsync();
                if (order.Id > 0)
                    return new Response(true, "Order place successfully");
                else return new Response(false, "Error occured while placing order");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if(order is null)
                    return new Response(false, "Order not found");

                orderDbContext.Orders.Remove(entity);
                await orderDbContext.SaveChangesAsync();
                return new Response(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occured while deleting order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await orderDbContext.Orders.FindAsync(id);
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occured while geting order");
            }
        }

        public async Task<Order> FindByNameAsync(string name)
        {
            try
            {
                var order = await orderDbContext.Orders.FindAsync(name);
                return order!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occured while geting order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders =await orderDbContext.Orders.AsNoTracking().ToListAsync();
                return orders;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occured while geting order");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await orderDbContext.Orders.Where(predicate).FirstOrDefaultAsync()!;
                return order!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occured while geting order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrderAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await orderDbContext.Orders.Where(predicate).ToListAsync()!;
                return orders!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occured while geting order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await orderDbContext.Orders.FindAsync(entity.Id);
                if (order is null)
                    return new Response(false, "order not found");

                orderDbContext.Entry(order).State = EntityState.Detached;
                orderDbContext.Orders.Update(entity);
                await orderDbContext.SaveChangesAsync();
                return new Response(true, "Order updated successfullt");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occured while placing order");
            }
        }
    }
}
