﻿using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.conversion
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quntity,
            Price = product.Price,

        };
        
        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDTO
                    (
                    product!.Id,
                    product.Name!,
                    product.Quantity,
                    product.Price);

                return (singleProduct, null);
            }

            if (product is null ||products is not null)
            {
                var _products = products.Select(p=>
                    new ProductDTO(p.Id,p.Name!,p.Quantity,p.Price)).ToList();

                return (null, _products);
            }

            return (null,null);
        }
    }
}
