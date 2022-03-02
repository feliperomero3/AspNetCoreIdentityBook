using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Identity
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        // set this field to false to disable the web service check.
        private const bool _remoteCheck = true;

        public IEnumerable<string> CommonPasswords => new[]
        {
            "123456", "123456789", "qwerty", "password", "1111111", "12345678",
            "abc123", "1234567", "password1", "12345", "1234567890", "123123",
            "000000", "Iloveyou", "1234", "1q2w3e4r5t", "Qwertyuiop", "123",
            "Monkey", "Dragon"
        };

        public async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            var errors = CheckTop20(password);

            if (_remoteCheck)
            {
                errors = errors.Concat(await CheckHaveIBeenPwned(password));
            }
            return !errors.Any()
                ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray());
        }

        private async Task<IEnumerable<IdentityError>> CheckHaveIBeenPwned(string password)
        {
            var hashedPassword = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", string.Empty);
            var firstSection = hashedPassword[0..5];
            var secondSection = hashedPassword[5..];

            var response = await new HttpClient().GetAsync($"https://api.pwnedpasswords.com/range/{firstSection}");

            var matchingHashes = await response.Content.ReadAsStringAsync();
            var matches = matchingHashes.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            string match = matches.FirstOrDefault(match => match.StartsWith(secondSection, StringComparison.CurrentCultureIgnoreCase));

            if (match == null)
            {
                return Enumerable.Empty<IdentityError>();
            }
            else
            {
                var count = long.Parse(match.Split(":")[1]);
                return new[] { new IdentityError { Description = $"Password has been compromised {count:N0} times." } };
            }
        }

        private IEnumerable<IdentityError> CheckTop20(string password)
        {
            return CommonPasswords.Any(commonPassword => string.Equals(commonPassword, password, StringComparison.CurrentCultureIgnoreCase))
                ? (new[] { new IdentityError { Description = "The top 20 passwords cannot be used." } })
                : Enumerable.Empty<IdentityError>();
        }
    }
}
