using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Data;
using IdentityApp.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ProductBindingTarget
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string Category { get; set; }

            [Required]
            public decimal Price { get; set; }
        }

        public IAsyncEnumerable<Product> GetProducts() => _context.Products;

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] ProductBindingTarget target)
        {
            if (ModelState.IsValid)
            {
                var product = new Product(target.Name, target.Description, target.Category, target.Price);

                await _context.AddAsync(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
