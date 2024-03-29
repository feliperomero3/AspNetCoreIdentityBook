using ExampleApp.Configurations;
using ExampleApp.Custom;
using ExampleApp.Identity;
using ExampleApp.Identity.Store;
using ExampleApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = IdentityConstants.ApplicationScheme;
                opts.AddScheme<ExternalAuthenticationHandler>("demoAuth", "Demo Service");
                opts.AddScheme<GoogleAuthenticationHandler>("google", "Google");
                opts.AddScheme<FacebookAuthenticationHandler>("facebook", "Facebook");
            })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.LoginPath = "/signin";
                options.AccessDeniedPath = $"/signin/{StatusCodes.Status403Forbidden}";
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme)
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme)
            .AddCookie(IdentityConstants.ExternalScheme);

            services.AddSingleton<RoleStore>();
            services.AddSingleton<UserStore>();
            services.AddSingleton<RoleStoreInitializer>();
            services.AddSingleton<UserStoreInitializer>();
            services.AddSingleton<ILookupNormalizer, Normalizer>();
            services.AddSingleton<IUserStore<AppUser>>(sp => sp.GetRequiredService<UserStore>());
            //services.AddSingleton<IUserValidator<AppUser>, EmailValidator>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<SmsSender>();
            services.AddSingleton<IPasswordHasher<AppUser>, SimplePasswordHasher>();
            services.AddSingleton<IPasswordValidator<AppUser>, PasswordValidator>();
            services.AddSingleton<IRoleStore<AppRole>>(sp => sp.GetRequiredService<RoleStore>());

            services.AddIdentityCore<AppUser>(opts =>
            {
                // The TokenOptions class uses the DefaultProvider property as the value for the
                // ChangeEmailTokenProvider and EmailConfirmationTokenProvider configuration options. This means
                // you must use a custom name for your token generator and perform the additional step of setting the
                // EmailConfirmationTokenProvider and ChangeEmailTokenProvider options.
                opts.Tokens.EmailConfirmationTokenProvider = "SimpleEmail";
                opts.Tokens.ChangeEmailTokenProvider = "SimpleEmail";
                opts.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 8;
                opts.Lockout.MaxFailedAccessAttempts = 3;
                opts.SignIn.RequireConfirmedAccount = true;
            })
            .AddTokenProvider<EmailConfirmationTokenGenerator>("SimpleEmail")
            
            //You can use TokenOptions.DefaultPhoneProvider and your generator
            //will be used as the default generator for phone number confirmations.
            .AddTokenProvider<PhoneConfirmationTokenGenerator>(TokenOptions.DefaultPhoneProvider)            
            .AddTokenProvider<TwoFactorSignInTokenGenerator>(IdentityConstants.TwoFactorUserIdScheme)
            .AddTokenProvider<AuthenticatorTokenProvider<AppUser>>(TokenOptions.DefaultAuthenticatorProvider)
            .AddSignInManager()
            .AddRoles<AppRole>();

            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();
            services.AddSingleton<IRoleValidator<AppRole>, RoleValidator>();
            services.AddSingleton<IUserConfirmation<AppUser>, UserConfirmation>();

            services.AddAuthorization(options => AuthorizationPolicies.AddPolicies(options));

            services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddOptions<ExternalAuthenticationOptions>();
            services.AddHttpClient();

            services.Configure<GoogleOptions>(Configuration.GetSection(GoogleOptions.OptionKey));
            services.Configure<FacebookOptions>(Configuration.GetSection(FacebookOptions.OptionKey));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

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
