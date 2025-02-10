using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly ProductDbContext productDbContext;
        public ProductRepository(ProductDbContext context)
        {
                productDbContext = context;
        }
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProduct =await GetByAsync(n=>n.Name!.Equals(entity.Name));
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} alredy added");
                }

                var currentEntity = productDbContext.products.Add(entity).Entity;
                await productDbContext.SaveChangesAsync();

                if (currentEntity != null)
                {
                    return new Response(true, $"{entity.Name} added successfully");
                }
                else
                {
                    return new Response(true, $"Error occured while adding {entity.Name}");
                }

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occured");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product != null)
                {
                    productDbContext.products.Remove(product);
                    productDbContext.SaveChanges();
                    return new Response(false, $"{entity.Name} Product Deleted");
                }
                else 
                {
                    return new Response(false, "Product not found");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occured");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await productDbContext.products.FindAsync(id); 
                return product is not null ? product : null!;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured");
            }
        }

        public Task<Product> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await productDbContext.products.AsNoTracking().ToListAsync();
                return products is not null ? products : Enumerable.Empty<Product>();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var products = await productDbContext.products.Where(predicate).FirstOrDefaultAsync()!;
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found");

                // Update properties instead of replacing the entity
                product.Name = entity.Name;
                product.Price = entity.Price;
                product.Quantity = entity.Quantity;
                // Add other properties as needed

                await productDbContext.SaveChangesAsync();

                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while updating");
            }
        }

    }
}
