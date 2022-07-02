using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ExampleApp.Custom
{
    // An authentication handler that returns canned results, without actually performing authentication.
    // This is a minimal implementation of the IAuthenticationHandler interface that contains just enough
    // code to compile.
    public class ExternalAuthenticationHandler : IAuthenticationHandler
    {
        private HttpContext _context;
        private AuthenticationScheme _scheme;

        public ExternalAuthenticationHandler(IOptions<ExternalAuthenticationOptions> options)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public ExternalAuthenticationOptions Options { get; private set; }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            // TODO: authentication implementation

            var identity = new ClaimsIdentity(_scheme.Name);

            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "333dc12a-f941-4aec-a257-0c25a707bbff"),
                new Claim(ClaimTypes.Email, "alice@example.com"),
                new Claim(ClaimTypes.Name, "Alice")
            });

            var principal = new ClaimsPrincipal(identity);

            // The IdentityConstants.ExternalScheme is used to sign in the external user
            // to prepare for the next phase in the process.
            // The other arguments to the SignInAsync method are the ClaimsPrincipal object
            // and the AuthenticationProperties object, which ensures that the state data
            // received by the handler is preserved.
            // Once the external user has been signed in, the handler issues a redirection
            // to the URL specified by the AuthenticationProperties parameter's RedirectUri method.
            await _context.SignInAsync(IdentityConstants.ExternalScheme, principal, properties);

            _context.Response.Redirect(properties.RedirectUri);
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;

            return Task.CompletedTask;
        }
    }
}