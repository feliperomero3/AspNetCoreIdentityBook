using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class UserConfirmation : IUserConfirmation<AppUser>
    {
        public async Task<bool> IsConfirmedAsync(UserManager<AppUser> manager, AppUser user)
        {
            var isInAdminRole = await manager.IsInRoleAsync(user, "Administrator");

            var claims = await manager.GetClaimsAsync(user);

            var isUserConfirmed = claims.Any(claim => claim.Type == "UserConfirmed" && string.Compare(claim.Value, "true", true) == 0);

            return isInAdminRole || isUserConfirmed;
        }
    }
}