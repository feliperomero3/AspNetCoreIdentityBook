using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ExampleApp.Custom
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(AuthorizationOptions options)
        {
            AddFallbackPolicy(options);
            AddDefaultPolicy(options);
            AddNamedPolicy(options);
        }

        private static void AddFallbackPolicy(AuthorizationOptions options)
        {
            var requirements = new IAuthorizationRequirement[] {
                new RolesAuthorizationRequirement(new[] { "User", "Administrator" }),
                new AssertionRequirement(context => !string.Equals(context.User.Identity.Name, "Bob"))
            };
            var schemes = new[] { UsersAndClaims.Schemes[0] };

            options.FallbackPolicy = new AuthorizationPolicy(requirements, schemes);
        }

        private static void AddDefaultPolicy(AuthorizationOptions options)
        {
            var requirements = new IAuthorizationRequirement[] {
                new RolesAuthorizationRequirement(new[] { "Administrator" }),
                new AssertionRequirement(context => !string.Equals(context.User.Identity.Name, "Bob"))
            };
            var schemes = Enumerable.Empty<string>();

            options.DefaultPolicy = new AuthorizationPolicy(requirements, schemes);
        }

        private static void AddNamedPolicy(AuthorizationOptions options)
        {
            var requirements = new IAuthorizationRequirement[] {
                new RolesAuthorizationRequirement(new[] { "User" }),
                new AssertionRequirement(context => !string.Equals(context.User.Identity.Name, "Bob"))
            };
            var schemes = Enumerable.Empty<string>();

            options.AddPolicy("UsersExceptBob", new AuthorizationPolicy(requirements, schemes));
        }
    }
}
