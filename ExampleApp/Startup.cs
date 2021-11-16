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
            services.AddAuthentication(options =>
            {
                options.AddScheme<AuthenticationHandler>(ExampleAppConstants.Scheme, ExampleAppConstants.AuthenticationType);
                options.DefaultScheme = ExampleAppConstants.Scheme;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMiddleware<RoleMemberships>();

            app.UseRouting();

            app.UseMiddleware<ClaimsReporter>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/secret", SecretEndpoint.Endpoint).WithDisplayName("secret")
                    .RequireAuthorization("RequireAdministratorRole");
                endpoints.MapRazorPages();
            });
        }
    }
}
