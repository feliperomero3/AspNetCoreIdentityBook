﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class RoleStore : IRoleStore<AppRole>
    {
        private readonly ConcurrentDictionary<string, AppRole> _roles = new ConcurrentDictionary<string, AppRole>();
        private bool _disposed;

        private static IdentityResult Error => IdentityResult.Failed(new IdentityError
        {
            Code = "StorageFailure",
            Description = "Role Store Error"
        });

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
            throw new NotImplementedException();
        }

        public Task<AppRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}