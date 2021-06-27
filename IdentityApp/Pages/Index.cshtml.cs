using System.Collections.Generic;
using System.Linq;
using IdentityApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SportsStore.Entities;

namespace IdentityApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Product> Products { get; set; }

        public PageResult OnGet()
        {
            Products = _context.Products.ToArray();

            return Page();
        }
    }
}
