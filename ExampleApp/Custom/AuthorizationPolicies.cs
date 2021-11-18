using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ExampleApp.Custom
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(AuthorizationOptions options)
        {
            var requirements = new[] { new NameAuthorizationRequirement("Bob") };
            var schemes = Enumerable.Empty<string>();

            options.FallbackPolicy = new AuthorizationPolicy(requirements, schemes);
        }
    }
}
