﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class UserStore :
        IQueryableUserStore<AppUser>,
        IUserEmailStore<AppUser>,
        IUserPhoneNumberStore<AppUser>,
        IUserClaimStore<AppUser>,
        IEqualityComparer<Claim>,
        IUserRoleStore<AppUser>,
        IUserSecurityStampStore<AppUser>,
        IUserPasswordStore<AppUser>,
        IUserLockoutStore<AppUser>,
        IUserTwoFactorStore<AppUser>,
        IUserAuthenticatorKeyStore<AppUser>,
        IUserTwoFactorRecoveryCodeStore<AppUser>,
        IUserLoginStore<AppUser>,
        IUserAuthenticationTokenStore<AppUser>
    {
        private readonly ConcurrentDictionary<string, AppUser> _users = new ConcurrentDictionary<string, AppUser>();
        private readonly Dictionary<string, IEnumerable<RecoveryCode>> _recoveryCodes = new Dictionary<string, IEnumerable<RecoveryCode>>();
        private readonly ILookupNormalizer _normalizer;

        public UserStore(ILookupNormalizer normalizer, IPasswordHasher<AppUser> passwordHasher)
        {
            _normalizer = normalizer;
        }

        private bool _disposed;

        private static IdentityResult Error => IdentityResult.Failed(new IdentityError
        {
            Code = "StorageFailure",
            Description = "User Store Error"
        });

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            return !_users.ContainsKey(user.Id) && _users.TryAdd(user.Id, user)
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(Error);
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (_users.ContainsKey(user.Id))
            {
                _users[user.Id].UpdateFrom(user);
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (_users.ContainsKey(user.Id) && _users.TryRemove(user.Id, out user))
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _users.ContainsKey(userId) ? _users[userId].Clone() : null;

            return Task.FromResult(user);
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _users.Values.FirstOrDefault(user => user.NormalizedUserName == normalizedUserName);

            return Task.FromResult(user?.Clone());
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName = normalizedName);
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName = userName);
        }

        public IQueryable<AppUser> Users => _users.Values.Select(user => user.Clone()).AsQueryable();

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            user.EmailAddress = email;

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailAddress);
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IsEmailAddressConfirmed);
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.IsEmailAddressConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.NormalizedEmailAddress == normalizedEmail));
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmailAddress);
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmailAddress = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.IsPhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Claims);
        }

        public Task AddClaimsAsync(AppUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user.Claims == null)
            {
                user.Claims = new List<Claim>();
            }
            foreach (var claim in claims)
            {
                user.Claims.Add(claim);
            }
            return Task.CompletedTask;
        }

        public async Task ReplaceClaimAsync(AppUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await RemoveClaimsAsync(user, new[] { claim }, cancellationToken);
            user.Claims.Add(newClaim);
        }

        public Task RemoveClaimsAsync(AppUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in user.Claims.Intersect(claims, this).ToList())
            {
                user.Claims.Remove(claim);
            }
            return Task.CompletedTask;
        }

        public Task<IList<AppUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.Where(u => u.Claims.Any(c => Equals(c, claim))).ToList() as IList<AppUser>);
        }

        public bool Equals(Claim x, Claim y)
        {
            return x.Type == y.Type && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] Claim claim)
        {
            return claim.Type.GetHashCode() + claim.Value.GetHashCode();
        }

        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            return AddClaimsAsync(user, GetClaim(roleName), cancellationToken);
        }

        private static IEnumerable<Claim> GetClaim(string roleName)
        {
            return new[] { new Claim(ClaimTypes.Role, roleName) };
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            var claimsToDelete = (await GetClaimsAsync(user, cancellationToken))
                .Where(claim => claim.Type == ClaimTypes.Role && _normalizer.NormalizeName(claim.Value) == roleName);

            await RemoveClaimsAsync(user, claimsToDelete, cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            return (await GetClaimsAsync(user, cancellationToken))
                 .Where(claim => claim.Type == ClaimTypes.Role)
                 .Distinct().Select(claim => _normalizer.NormalizeName(claim.Value))
                 .ToList();
        }

        public async Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            return (await GetRolesAsync(user, cancellationToken)).Any(role => _normalizer.NormalizeName(role) == roleName);
        }

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return GetUsersForClaimAsync(new Claim(ClaimTypes.Role, roleName), cancellationToken);
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockOutEndDate);
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockOutEndDate = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(++user.FailedSignInAttemptsCount);
        }

        public Task ResetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            user.FailedSignInAttemptsCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.FailedSignInAttemptsCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.CanUserBeLockedOut);
        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.CanUserBeLockedOut = enabled;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.IsTwoFactorAuthenticationEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IsTwoFactorAuthenticationEnabled);
        }

        public Task SetAuthenticatorKeyAsync(AppUser user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;
            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task ReplaceCodesAsync(AppUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            _recoveryCodes[user.Id] = recoveryCodes.Select(rc => new RecoveryCode { Code = rc, Redeemed = false });

            return Task.CompletedTask;
        }

        public async Task<bool> RedeemCodeAsync(AppUser user, string code, CancellationToken cancellationToken)
        {
            var codes = (await GetCodesAsync(user)).ToList();
            var recoveryCode = codes.FirstOrDefault(rc => !rc.Redeemed && rc.Code == code);

            if (recoveryCode is not null)
            {
                recoveryCode.Redeemed = true;

                _recoveryCodes.Remove(user.Id);

                _recoveryCodes.Add(user.Id, codes);

                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<int> CountCodesAsync(AppUser user, CancellationToken cancellationToken)
        {
            var codes = await GetCodesAsync(user);
            var unRedeemedCodes = codes.Where(c => !c.Redeemed);

            return await Task.FromResult(unRedeemedCodes.Count());
        }

        public Task<IEnumerable<RecoveryCode>> GetCodesAsync(AppUser user)
        {
            var codes = _recoveryCodes.ContainsKey(user.Id)
                ? _recoveryCodes[user.Id]
                : Enumerable.Empty<RecoveryCode>();

            return Task.FromResult(codes);
        }

        public Task AddLoginAsync(AppUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user.UserLogins == null)
            {
                user.UserLogins = new List<UserLoginInfo>();
            }

            user.UserLogins.Add(login);

            return Task.CompletedTask;
        }

        public async Task RemoveLoginAsync(AppUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userLogins = await GetLoginsAsync(user, cancellationToken);

            if (userLogins is not null)
            {
                user.UserLogins = userLogins.Where(login => !login.LoginProvider.Equals(loginProvider) && !login.ProviderKey.Equals(providerKey)).ToList();
            }
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserLogins ?? new List<UserLoginInfo>());
        }

        public Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var usersWithLogins = Users.Where(u => u.UserLogins != null && u.UserLogins.Count > 0);

            var userFound = usersWithLogins?.FirstOrDefault(u => u.UserLogins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));

            return Task.FromResult(userFound);
        }

        public Task SetTokenAsync(AppUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.AuthTokens is null)
            {
                user.AuthTokens = new List<(string, AuthenticationToken)>();
            }
            user.AuthTokens.Add((loginProvider, new AuthenticationToken
            {
                Name = name,
                Value = value
            }));
            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(AppUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.AuthTokens is not null)
            {
                user.AuthTokens = user.AuthTokens.Where(t => t.provider != loginProvider && t.token.Name != name).ToList();
            }
            return Task.CompletedTask;
        }

        public Task<string> GetTokenAsync(AppUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenValue = user.AuthTokens?.FirstOrDefault(t => t.provider == loginProvider && t.token.Name == name).token.Value;

            return Task.FromResult(tokenValue);
        }
    }
}
