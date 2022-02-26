using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class SimplePasswordHasher : IPasswordHasher<AppUser>
    {
        private readonly ILookupNormalizer _normalizer;

        public SimplePasswordHasher(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public string HashPassword(AppUser user, string password)
        {
            HMACSHA256 hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(user.Id));

            return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
        {
            return HashPassword(user, providedPassword).Equals(hashedPassword)
                 ? PasswordVerificationResult.Success
                 : PasswordVerificationResult.Failed;
        }
    }
}
