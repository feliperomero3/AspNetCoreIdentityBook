using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ExampleApp.Custom
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(AuthorizationOptions options)
        {
            var requirements = new IAuthorizationRequirement[] {
                new RolesAuthorizationRequirement(new[] { "User", "Administrator" }),
                new AssertionRequirement(context => !string.Equals(context.User.Identity.Name, "Bob"))
            };
            var schemes = Enumerable.Empty<string>();

            options.FallbackPolicy = new AuthorizationPolicy(requirements, schemes);
        }
    }
}
