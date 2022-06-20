using ExampleApp.Identity.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var roleStore = services.GetRequiredService<RoleStore>();
            var roles = services.GetRequiredService<RoleStoreInitializer>();

            roles.SeedStore(roleStore);

            var userStore = services.GetRequiredService<UserStore>();
            var users = services.GetRequiredService<UserStoreInitializer>();

            users.SeedStore(userStore);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
