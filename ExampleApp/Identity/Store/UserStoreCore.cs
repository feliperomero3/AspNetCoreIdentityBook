using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class UserStore : IQueryableUserStore<AppUser>, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>, IUserClaimStore<AppUser>, IEqualityComparer<Claim>
    {
        private readonly ConcurrentDictionary<string, AppUser> users = new ConcurrentDictionary<string, AppUser>();
        private readonly ILookupNormalizer _normalizer;

        public UserStore(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
            SeedStore();
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

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (!users.ContainsKey(user.Id) && users.TryAdd(user.Id, user))
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (users.ContainsKey(user.Id))
            {
                users[user.Id].UpdateFrom(user);
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (users.ContainsKey(user.Id) && users.TryRemove(user.Id, out user))
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = users.ContainsKey(userId) ? users[userId].Clone() : null;

            return Task.FromResult(user);
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = users.Values.FirstOrDefault(user => user.NormalizedUserName == normalizedUserName);

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

        public IQueryable<AppUser> Users => users.Values.Select(user => user.Clone()).AsQueryable();

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

        private void SeedStore()
        {
            var customData = new Dictionary<string, (string food, string hobby)>
            {
                { "Alice", ("Pizza", "Running") },
                { "Bob", ("Ice Cream", "Cinema") },
                { "Charlie", ("Burgers", "Cooking") }
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
                    Hobby = customData[name].hobby
                };
                user.Claims = UsersAndClaims.UserData[user.UserName].Select(role => new Claim(ClaimTypes.Role, role)).ToList();
                users.TryAdd(user.Id, user);
            }
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
    }
}
