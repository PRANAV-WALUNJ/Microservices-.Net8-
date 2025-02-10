using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.conversion;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _product;

        public ProductController(IProduct product)
        {
            _product = product;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _product.GetAllAsync();
            if (!products.Any())  // Corrected condition
                return NotFound("No product found in database");

            var (_, list) = ProductConversion.FromEntity(null!, products);

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _product.FindByIdAsync(id);
            if (product == null)
                return NotFound("Not found");

            var (_item,_) =ProductConversion.FromEntity(product,null!);
            return Ok(_item);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await _product.CreateAsync(getEntity);
            return Ok(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await _product.UpdateAsync(getEntity);
            return Ok(response);

        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            var getEntity = ProductConversion.ToEntity(product);
            var response = await _product.DeleteAsync(getEntity);
            return Ok(response);

        }
    }

}
