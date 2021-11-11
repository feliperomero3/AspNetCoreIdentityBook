using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class ClaimsReporter
    {
        private readonly RequestDelegate _next;

        public ClaimsReporter(RequestDelegate requestDelegate) => _next = requestDelegate;

        public async Task Invoke(HttpContext context)
        {
            var p = context.User;

            Console.WriteLine($"User: {p.Identity.Name}");
            Console.WriteLine($"Authenticated: {p.Identity.IsAuthenticated}");
            Console.WriteLine($"Authentication Type: {p.Identity.AuthenticationType}");
            Console.WriteLine($"Identities: {p.Identities.Count()}");

            foreach (var identity in p.Identities)
            {
                Console.WriteLine($"Auth type: {identity.AuthenticationType}, {identity.Claims.Count()} claims");

                foreach (var claim in identity.Claims)
                {
                    Console.WriteLine($"Type: {GetName(claim.Type)}, Value: {claim.Value}, Issuer: {claim.Issuer}");
                }
            }

            await _next(context);
        }

        private static string GetName(string claimType) => Path.GetFileName(new Uri(claimType).LocalPath);
    }
}
