using System.Linq;
using SportsStore.Entities;

namespace IdentityApp.Data
{
    internal static class ApplicationDbSeedData
    {
        internal static void SeedDatabase(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return; // Db has been seeded
            }

            var p1 = new Product("Kayak", "A boat for one person", "Water sports", 275);
            var p2 = new Product("Life Jacket", "Protective and fashionable", "Water sports", 48.95m);
            var p3 = new Product("Football Ball", "FIFA-approved size and weight", "Football", 19.5m);
            var p4 = new Product("Corner Flags", "Give your pitch a professional touch", "Football", 34.95m);
            var p5 = new Product("Stadium", "Flat-packed 35,000-seat stadium", "Football", 79500);
            var p6 = new Product("Thinking Cap", "Improve brain efficiency by 75%", "Chess", 16);
            var p7 = new Product("Unsteady Chair", "Secretly give your opponent a disadvantage", "Chess", 29.95m);
            var p8 = new Product("Human Chess Board", "A fun game for the family", "Chess", 75);
            var p9 = new Product("Bling-Bling King", "Gold-plated, diamond-studded King", "Chess", 1200);

            context.AddRange(p1, p2, p3, p4, p5, p6, p7, p8, p9);

            context.SaveChanges();
        }
    }
}
