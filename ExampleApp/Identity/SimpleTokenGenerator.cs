using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    /* You should not use this approach in real projects and should instead rely on the tokens generators
     * that ASP.NET Core Identity provides. 
     */
    public abstract class SimpleTokenGenerator : IUserTwoFactorTokenProvider<AppUser>
    {
        protected virtual int CodeLength { get; } = 6;

        public virtual Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(manager.SupportsUserSecurityStamp);
        }

        public virtual Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(GenerateCode(purpose, user));
        }

        /* The inclusion of the purpose ensures that a token is valid for only one type of confirmation.
         * The inclusion of the security stamp ensures that tokens are invalidated when a change is made to the user object,
         * ensuring that the token can be used only until a change is made in the user store.
         */
        public virtual Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(GenerateCode(purpose, user).Equals(token));
        }

        protected virtual string GenerateCode(string purpose, AppUser user)
        {
            var hashAlgorithm = new HMACSHA1(Encoding.UTF8.GetBytes(user.SecurityStamp));

            byte[] hashCode = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(GetData(purpose, user)));

            return BitConverter.ToString(hashCode[^CodeLength..]).Replace("-", "");
        }

        protected virtual string GetData(string purpose, AppUser user) => purpose + user.SecurityStamp;
    }

    public class EmailConfirmationTokenGenerator : SimpleTokenGenerator
    {
        protected override int CodeLength => 12;

        public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return await base.CanGenerateTwoFactorTokenAsync(manager, user)
                && !string.IsNullOrEmpty(user.EmailAddress)
                && !user.IsEmailAddressConfirmed;
        }
    }

    public class PhoneConfirmationTokenGenerator : SimpleTokenGenerator
    {
        /* The email and SMS confirmation tokens are different lengths. This is not a requirement, but users who
         * receive tokens over SMS will often have to type them into an HTML form, and it is important to make that
         * process easy.
         */
        protected override int CodeLength => 3;

        public async override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return await base.CanGenerateTwoFactorTokenAsync(manager, user)
                && !string.IsNullOrEmpty(user.PhoneNumber)
                && !user.IsPhoneNumberConfirmed;
        }
    }
}
