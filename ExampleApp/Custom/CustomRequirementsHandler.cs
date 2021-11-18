using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ExampleApp.Custom
{
    public class CustomRequirementsHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.OfType<CustomRequirement>().ToList())
            {
                if (context.User.Identities.Any(identity => string.Equals(identity.Name, requirement.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
