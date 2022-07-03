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
    // This is a minimal implementation of the IAuthenticationHandler interface that contains just enough
    // code to compile.
    public class ExternalAuthenticationHandler : IAuthenticationHandler
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