using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string EmailAddress { get; set; }

        public string NormalizedEmailAddress { get; set; }

        public bool IsEmailAddressConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        public string FavoriteFood { get; set; }

        public string Hobby { get; set; }

        public IList<Claim> Claims { get; set; }

        public string SecurityStamp { get; set; }

        public string PasswordHash { get; set; }

        public bool CanUserBeLockedOut { get; set; } = true;

        public int FailedSignInAttemptsCount { get; set; }

        public DateTimeOffset? LockOutEndDate { get; set; }

        public bool IsTwoFactorAuthenticationEnabled { get; set; }

        public bool IsAuthenticatorEnabled { get; set; }

        public string AuthenticatorKey { get; set; }

        public IList<UserLoginInfo> UserLogins { get; set; }

        // Identity provides the AuthenticationToken class, which defines Name and Value properties.
        // To store tokens, I need to be able to keep track of the source of each token, so I have used
        // a list of (string, AuthenticationToken) tuples for simplicity.
        public IList<(string provider, AuthenticationToken token)> AuthTokens { get; set; }
    }
}
