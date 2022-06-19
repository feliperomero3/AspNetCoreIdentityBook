using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class RoleStore : IRoleStore<AppRole>, IQueryableRoleStore<AppRole>
    {
        private readonly ConcurrentDictionary<string, AppRole> _roles = new ConcurrentDictionary<string, AppRole>();
        private readonly ILookupNormalizer _normalizer;
        private bool _disposed;

        public RoleStore(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
            SeedStore();
        }

        private void SeedStore()
        {
            var roleData = new[] { "Administrator", "User", "Sales", "Support" };
            var idCounter = 0;

            foreach (string roleName in roleData)
            {
                AppRole role = new AppRole
                {
                    Id = (++idCounter).ToString(),
                    Name = roleName,
                    NormalizedName = _normalizer.NormalizeName(roleName)
                };

                _roles.TryAdd(role.Id, role);
            }
        }

        private static IdentityResult Error => IdentityResult.Failed(new IdentityError
        {
            Code = "StorageFailure",
            Description = "Role Store Error"
        });

        public IQueryable<AppRole> Roles => _roles.Values.Select(role => role.Clone()).AsQueryable();

        public Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            return !_roles.ContainsKey(role.Id) && _roles.TryAdd(role.Id, role)
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(Error);
        }

        public Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            if (_roles.ContainsKey(role.Id))
            {
                _roles[role.Id].UpdateFrom(role);

                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(Error);
        }

        public Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            return _roles.ContainsKey(role.Id) && _roles.TryRemove(role.Id, out role)
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(Error);
        }

        public Task<AppRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_roles.ContainsKey(roleId) ? _roles[roleId].Clone() : null);
        }

        public Task<AppRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var appRole = _roles.Values.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);
            return Task.FromResult(appRole?.Clone());
        }

        public Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}