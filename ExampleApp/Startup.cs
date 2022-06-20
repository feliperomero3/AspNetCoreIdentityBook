using ExampleApp.Custom;
using ExampleApp.Identity;
using ExampleApp.Identity.Store;
using ExampleApp.Services;
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
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = "/signin";
                    options.AccessDeniedPath = $"/signin/{StatusCodes.Status403Forbidden}";
                });

            services.AddSingleton<ILookupNormalizer, Normalizer>();
            services.AddSingleton<IUserStore<AppUser>, UserStore>();
            services.AddSingleton<IUserValidator<AppUser>, EmailValidator>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<SmsSender>();
            services.AddSingleton<IPasswordHasher<AppUser>, SimplePasswordHasher>();
            services.AddSingleton<IPasswordValidator<AppUser>, PasswordValidator>();
            services.AddSingleton<IRoleStore<AppRole>, RoleStore>();
            services.AddSingleton<RoleStoreInitializer>();
            services.AddSingleton<UserStoreInitializer>();

            services.AddIdentityCore<AppUser>(opts =>
            {
                /* The TokenOptions class uses the DefaultProvider property as the value for the
                 * ChangeEmailTokenProvider and EmailConfirmationTokenProvider configuration options. This means
                 * you must use a custom name for your token generator and perform the additional step of setting the
                 * EmailConfirmationTokenProvider and ChangeEmailTokenProvider options.
                 */
                opts.Tokens.EmailConfirmationTokenProvider = "SimpleEmail";
                opts.Tokens.ChangeEmailTokenProvider = "SimpleEmail";
                opts.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 8;
            })
            .AddTokenProvider<EmailConfirmationTokenGenerator>("SimpleEmail")
            /* You can use TokenOptions.DefaultPhoneProvider and your generator
             * will be used as the default generator for phone number confirmations.
             */
            .AddTokenProvider<PhoneConfirmationTokenGenerator>(TokenOptions.DefaultPhoneProvider)
            .AddSignInManager()
            .AddRoles<AppRole>();

            services.AddSingleton<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();
            services.AddAuthorization(options => AuthorizationPolicies.AddPolicies(options));

            services.AddRazorPages();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

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
