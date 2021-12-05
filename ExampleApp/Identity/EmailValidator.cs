using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class EmailValidator : IUserValidator<AppUser>
    {
        private static readonly string[] AllowedDomains = new[] { "example.com", "acme.com" };
        private static readonly IdentityError error = new() { Description = "Email address domain not allowed." };
        private readonly ILookupNormalizer _normalizer;

        public EmailValidator(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var normalizedEmail = _normalizer.NormalizeEmail(user.EmailAddress);

            if (AllowedDomains.Any(domain => normalizedEmail.EndsWith($"@{domain}")))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(error));
        }
    }
}
