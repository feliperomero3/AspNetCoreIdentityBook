using System.Collections.Generic;
using System.Linq;
using IdentityApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Entities;

namespace IdentityApp.Pages
{
    public class StoreModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StoreModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> Products { get; set; }

        public void OnGet()
        {
            Products = _context.Products.AsEnumerable();
        }
    }
}
