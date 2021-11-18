using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ExampleApp.Custom
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(AuthorizationOptions options)
        {
            var requirements = new[] { new CustomRequirement() { Name = "Bob" } };
            var schemes = Enumerable.Empty<string>();

            options.FallbackPolicy = new AuthorizationPolicy(requirements, schemes);
        }
    }
}
