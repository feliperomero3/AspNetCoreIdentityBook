using ExampleApp.Custom;
using ExampleApp.Identity;
using ExampleApp.Identity.Store;
using ExampleApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.AccessDeniedPath = $"/signin/{StatusCodes.Status403Forbidden}";
                });

            services.AddIdentityCore<AppUser>();
            services.AddSingleton<ILookupNormalizer, Normalizer>();
            services.AddSingleton<IUserStore<AppUser>, UserStore>();
            services.AddSingleton<IUserValidator<AppUser>, EmailValidator>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<SmsSender>();

            services.AddAuthorization(options => AuthorizationPolicies.AddPolicies(options));

            services.AddRazorPages();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseMiddleware<RoleMemberships>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/Secret");
            });
        }
    }
}
