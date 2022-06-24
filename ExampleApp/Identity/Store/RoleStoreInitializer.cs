using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class RoleStoreInitializer
    {
        private readonly ILookupNormalizer _normalizer;

        public RoleStoreInitializer(ILookupNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public void SeedStore(RoleStore roleStore)
        {
            var roleData = new[] { "Administrator", "User", "Sales", "Support" };
            var claims = new Dictionary<string, IEnumerable<Claim>>
            {
                { "Administrator", new[] { new Claim("AccessUserData", "true"), new Claim(ClaimTypes.Role, "Support") } },
                { "Support", new[] { new Claim(ClaimTypes.Role, "User" ) } }
            };
            var idCounter = 0;

            foreach (var roleName in roleData)
            {
                var role = new AppRole
                {
                    Id = (++idCounter).ToString(),
                    Name = roleName,
                    NormalizedName = _normalizer.NormalizeName(roleName)
                };
                if (claims.ContainsKey(roleName))
                {
                    role.Claims = claims[roleName].ToArray();
                }

                roleStore.CreateAsync(role).Wait();
            }
        }
    }
}