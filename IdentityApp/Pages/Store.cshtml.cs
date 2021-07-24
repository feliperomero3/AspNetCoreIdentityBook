using System.Collections.Generic;
using System.Linq;
using IdentityApp.Data;
using IdentityApp.Entities;
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

        public IEnumerable<Product> Products { get; set; }

        public void OnGet()
        {
            Products = _context.Products.AsEnumerable();
        }
    }
}
