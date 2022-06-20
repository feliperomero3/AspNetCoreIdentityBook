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
            var idCounter = 0;

            foreach (string roleName in roleData)
            {
                AppRole role = new AppRole
                {
                    Id = (++idCounter).ToString(),
                    Name = roleName,
                    NormalizedName = _normalizer.NormalizeName(roleName)
                };

                roleStore.CreateAsync(role).Wait();
            }
        }
    }
}