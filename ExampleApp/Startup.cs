using ExampleApp.Custom;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CustomAuthentication>();

            app.UseMiddleware<RoleMemberships>();

            app.UseRouting();

            app.UseMiddleware<ClaimsReporter>();

            app.UseMiddleware<CustomAuthorization>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/secret", SecretEndpoint.Endpoint).WithDisplayName("secret");
                endpoints.Map("/signin", CustomSignInAndSignOut.SignIn);
                endpoints.Map("/signout", CustomSignInAndSignOut.SignOut);
            });
        }
    }
}
