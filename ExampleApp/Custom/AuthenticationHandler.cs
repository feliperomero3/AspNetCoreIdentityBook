using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class AuthenticationHandler : IAuthenticationSignInHandler
    {
        private HttpContext _context;
        private AuthenticationScheme _scheme;
        private const string cookieKey = "authUser";

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            AuthenticateResult result;

            var user = _context.Request.Cookies[cookieKey];

            if (user != null)
            {
                var claim = new Claim(ClaimTypes.Name, user);
                var identity = new ClaimsIdentity(_scheme.Name);
                identity.AddClaim(claim);
                result = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), _scheme.Name));
            }
            else
            {
                result = AuthenticateResult.NoResult();
            }

            return Task.FromResult(result);
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = StatusCodes.Status403Forbidden;

            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;

            return Task.CompletedTask;
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            _context.Response.Cookies.Append(cookieKey, user.Identity.Name, new CookieOptions { Secure = true, HttpOnly = true });

            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            _context.Response.Cookies.Delete(cookieKey);

            return Task.CompletedTask;
        }
    }
}
