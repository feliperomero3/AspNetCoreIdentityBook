using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ExampleApp.Custom
{
    // An authentication handler that returns canned results, without actually performing authentication.
    // The authentication handler needs to be able to receive the redirected request. ASP.NET Core defines the
    // IAuthenticationRequestHandler interface, which is derived from the IAuthenticationHandler interface.
    public class ExternalAuthenticationHandler : IAuthenticationRequestHandler
    {
        private readonly IDataProtectionProvider _dataProtection;
        private HttpContext _context;

        public ExternalAuthenticationHandler(
            IOptions<ExternalAuthenticationOptions> options,
            IDataProtectionProvider dataProtection)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _dataProtection = dataProtection ?? throw new ArgumentNullException(nameof(dataProtection));
        }

        public ExternalAuthenticationOptions Options { get; protected set; }
        public PropertiesDataFormat PropertiesFormatter { get; protected set; }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.Redirect(await GetAuthenticationUrl(properties));
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        // The HandleRequestAsync method is called automatically by the ASP.NET Core authentication
        // middleware and allows authentication handlers to intercept requests without the need to create custom
        // middleware classes or endpoints.
        public async Task<bool> HandleRequestAsync()
        {
            if (_context.Request.Path.Equals(Options.RedirectPath))
            {
                var authCode = await GetAuthenticationCode();
                return true;
            }
            return false;
        }

        private Task<string> GetAuthenticationCode()
        {
            return Task.FromResult(_context.Request.Query["code"].ToString());
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _context = context;

            var purpose = typeof(ExternalAuthenticationOptions).FullName;
            var protector = _dataProtection.CreateProtector(purpose);

            PropertiesFormatter = new PropertiesDataFormat(protector);

            return Task.CompletedTask;
        }

        protected virtual Task<string> GetAuthenticationUrl(AuthenticationProperties properties)
        {
            var queryStringValues = new Dictionary<string, string>
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", Options.RedirectRoot + Options.RedirectPath },
                { "scope", Options.Scope },
                { "response_type", "code" },
                { "state", PropertiesFormatter.Protect(properties) }
            };

            return Task.FromResult(Options.AuthenticationUrl + QueryString.Create(queryStringValues));
        }
    }
}