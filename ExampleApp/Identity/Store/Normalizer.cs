using Microsoft.AspNetCore.Identity;

namespace ExampleApp.Identity.Store
{
    public class Normalizer : ILookupNormalizer
    {
        public string NormalizeEmail(string email)
        {
            if (email == null)
            {
                return null;
            }
            return email.Normalize().ToLowerInvariant();
        }

        public string NormalizeName(string name)
        {
            if (name == null)
            {
                return null;
            }
            return name.Normalize().ToLowerInvariant();
        }
    }
}
