using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity
{
    public class AppUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<AppUser>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AppUserClaimsPrincipalFactory(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.EmailAddress)
            });
            if (!string.IsNullOrEmpty(user.Hobby))
            {
                identity.AddClaim(new Claim("Hobby", user.Hobby));
            }
            if (!string.IsNullOrEmpty(user.FavoriteFood))
            {
                identity.AddClaim(new Claim("FavoriteFood", user.FavoriteFood));
            }
            if (user.Claims != null)
            {
                identity.AddClaims(user.Claims);
            }
            if (_userManager.SupportsUserRole && _roleManager.SupportsRoleClaims)
            {
                var roleNames = await _userManager.GetRolesAsync(user);

                foreach (var roleName in roleNames)
                {
                    var appRole = await _roleManager.FindByNameAsync(roleName);

                    if (appRole is not null && appRole.Claims is not null)
                    {
                        identity.AddClaims(appRole.Claims);
                    }
                }
            }
            return new ClaimsPrincipal(identity);
        }
    }
}
