using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class AuthorizationReporter
    {
        private readonly string[] _schemes = new string[] { "TestScheme" };
        private readonly RequestDelegate _next;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationReporter(RequestDelegate requestDelegate,
                IAuthorizationPolicyProvider provider,
                IAuthorizationService service)
        {
            _next = requestDelegate;
            _policyProvider = provider;
            _authorizationService = service;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                var results = new Dictionary<(string, string), bool>();
                var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;
                var authData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();

                var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authData);

                foreach (var principal in GetUsers())
                {
                    results[(principal.Identity.Name ?? "(No User)", principal.Identity.AuthenticationType)] =
                        allowAnonymous || policy == null || await AuthorizeUser(principal, policy);
                }

                context.Items["authReport"] = results;

                await endpoint.RequestDelegate(context);
            }
            else
            {
                await _next(context);
            }
        }

        private IEnumerable<ClaimsPrincipal> GetUsers() =>
            UsersAndClaims.GetUsers().Concat(new[] { new ClaimsPrincipal(new ClaimsIdentity()) });

        private async Task<bool> AuthorizeUser(ClaimsPrincipal cp, AuthorizationPolicy policy)
        {
            return UserSchemeMatchesPolicySchemes(cp, policy)
                && (await _authorizationService.AuthorizeAsync(cp, policy)).Succeeded;
        }

        private bool UserSchemeMatchesPolicySchemes(ClaimsPrincipal cp, AuthorizationPolicy policy)
        {
            return policy.AuthenticationSchemes?.Count == 0 || cp.Identities.Select(id => id.AuthenticationType)
                    .Any(auth => policy.AuthenticationSchemes.Any(scheme => scheme == auth));
        }
    }
}
