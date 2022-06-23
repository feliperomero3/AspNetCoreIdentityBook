using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class RoleValidator : IRoleValidator<AppRole>
    {
        public async Task<IdentityResult> ValidateAsync(RoleManager<AppRole> manager, AppRole role)
        {
            var errors = new IdentityError
            {
                Code = "PluralizeSingularizeRoleName",
                Description = "Names cannot be plural/singular of existing roles."
            };

            var existingRole = await manager.FindByNameAsync(role.Name.EndsWith("s") ? role.Name[0..^1] : role.Name + "s");

            return existingRole is null
                ? IdentityResult.Success
                : IdentityResult.Failed(errors);
        }
    }
}