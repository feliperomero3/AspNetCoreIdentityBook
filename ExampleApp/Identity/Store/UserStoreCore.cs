﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class UserStore : IQueryableUserStore<AppUser>
    {
        private readonly ConcurrentDictionary<string, AppUser> users = new ConcurrentDictionary<string, AppUser>();
        private readonly ILookupNormalizer _normalizer;

        public UserStore(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
            SeedStore();
        }

        private bool _disposed;

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

        private void SeedStore()
        {
            var idCounter = 0;
            foreach (var name in UsersAndClaims.Users)
            {
                var user = new AppUser
                {
                    Id = (++idCounter).ToString(),
                    UserName = name,
                    NormalizedUserName = _normalizer.NormalizeName(name)
                };
                users.TryAdd(user.Id, user);
            }
        }

        private static IdentityResult Error => IdentityResult.Failed(new IdentityError
        {
            Code = "StorageFailure",
            Description = "User Store Error"
        });

        public IQueryable<AppUser> Users => users.Values.Select(user => user.Clone()).AsQueryable();

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
    }
}
