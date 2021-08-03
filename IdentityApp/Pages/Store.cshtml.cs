using System.Collections.Generic;
using System.Linq;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages
{
    [Authorize]
    public class StoreModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StoreModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductModel> Products { get; set; }

        public void OnGet()
        {
            var products = _context.Products.AsEnumerable();

            Products = products.Select(p => new ProductModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price
            });
        }
    }
}
