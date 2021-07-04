using System.Collections.Generic;
using IdentityApp.Data;
using IdentityApp.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> Products { get; set; }

        public void OnGet()
        {
        }

        public void OnPost(long id)
        {
            Product p = _context.Find<Product>(id);
            if (p != null)
            {
                _context.Remove(p);
                _context.SaveChanges();
            }
        }
    }
}
