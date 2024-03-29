﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class UserStoreInitializer
    {
        private readonly ILookupNormalizer _normalizer;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public UserStoreInitializer(ILookupNormalizer normalizer, IPasswordHasher<AppUser> passwordHasher)
        {
            _normalizer = normalizer;
            _passwordHasher = passwordHasher;
        }

        public void SeedStore(UserStore userStore)
        {
            var customData = new Dictionary<string, (string food, string hobby)>
            {
                { "Alice", ("Pizza", "Running") },
                { "Bob", ("Ice Cream", "Cinema") },
                { "Charlie", ("Burgers", "Cooking") }
            };

            var twoFactorUsers = new[] { "Alice", "Charlie" };

            var authenticatorKeys = new Dictionary<string, string>
            {
                { "Alice", "A4GG2BNKJNKKFOKGZRGBVUYIAJCUHEW7" }
            };

            var recoveryCodes = new Dictionary<string, string[]>
            {
                { "Alice", new[] { "abcd1234", "abcd5678" } }
            };

            var idCounter = 0;

            static string EmailFromName(string name) => $"{name.ToLower()}@example.com";

            foreach (var name in UsersAndClaims.Users)
            {
                var user = new AppUser
                {
                    Id = (++idCounter).ToString(),
                    UserName = name,
                    NormalizedUserName = _normalizer.NormalizeName(name),
                    EmailAddress = EmailFromName(name),
                    NormalizedEmailAddress = _normalizer.NormalizeEmail(EmailFromName(name)),
                    IsEmailAddressConfirmed = true,
                    PhoneNumber = "123-4567",
                    IsPhoneNumberConfirmed = true,
                    FavoriteFood = customData[name].food,
                    Hobby = customData[name].hobby,
                    SecurityStamp = "InitialStamp",
                    IsTwoFactorAuthenticationEnabled = twoFactorUsers.Contains(name)
                };

                user.Claims = UsersAndClaims.UserData[user.UserName].Select(role => new Claim(ClaimTypes.Role, role)).ToList();
                user.PasswordHash = _passwordHasher.HashPassword(user, "MySecret1$");

                if (authenticatorKeys.ContainsKey(name))
                {
                    user.AuthenticatorKey = authenticatorKeys[name];
                    user.IsAuthenticatorEnabled = true;
                }

                userStore.CreateAsync(user).Wait();

                if (recoveryCodes.ContainsKey(name))
                {
                    userStore.ReplaceCodesAsync(user, recoveryCodes[name], CancellationToken.None).Wait();
                }
            }
        }
    }
}