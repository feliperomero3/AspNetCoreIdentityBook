using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ExampleApp.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExampleApp.Custom
{
    public class FacebookAuthenticationHandler : ExternalAuthenticationHandler
    {
        public FacebookAuthenticationHandler(
            IOptions<FacebookAuthenticationOptions> options,
            IDataProtectionProvider dataProtection,
            HttpClient httpClient,
            ILogger<FacebookAuthenticationHandler> logger,
            IOptions<FacebookOptions> facebookOptions) : base(options, dataProtection, httpClient, logger)
        {
            Options.ClientId = facebookOptions.Value.ClientId;
            Options.ClientSecret = facebookOptions.Value.ClientSecret;

            if (string.IsNullOrEmpty(Options.ClientSecret) || string.Equals("MyClientSecret", Options.ClientSecret))
            {
                logger.LogError("External Authentication Secret not set.");
            }
        }

        protected override IEnumerable<Claim> GetClaims(JsonDocument jsonDoc)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, jsonDoc.RootElement.GetString("id")),
                new Claim(ClaimTypes.Name, jsonDoc.RootElement.GetString("name")?.Replace(" ", "_")),
                new Claim(ClaimTypes.Email, jsonDoc.RootElement.GetString("email"))
            };
            return claims;
        }
    }
}