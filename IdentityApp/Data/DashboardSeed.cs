using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityApp.Data
{
    public static class DashboardSeed
    {
        public static void SeedUserStoreForDashboard(IServiceScope scope)
        {
            SeedStore(scope).GetAwaiter().GetResult();
        }

        private async static Task SeedStore(IServiceScope scope)
        {
            var config = scope.ServiceProvider.GetService<IConfiguration>();
            var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            var roleName = config["Dashboard:Role"] ?? "Dashboard";
            var userName = config["Dashboard:User"] ?? "admin@example.com";
            var password = config["Dashboard:Password"] ?? "mysecret";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var dashboardUser = await userManager.FindByEmailAsync(userName);

            if (dashboardUser == null)
            {
                dashboardUser = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(dashboardUser);

                dashboardUser = await userManager.FindByEmailAsync(userName);

                await userManager.AddPasswordAsync(dashboardUser, password);
            }

            if (!await userManager.IsInRoleAsync(dashboardUser, roleName))
            {
                await userManager.AddToRoleAsync(dashboardUser, roleName);
            }
        }
    }
}
