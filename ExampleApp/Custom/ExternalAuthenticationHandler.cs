using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExampleApp.Custom
{
    // An authentication handler that returns canned results, without actually performing authentication.
    // The authentication handler needs to be able to receive the redirected request. ASP.NET Core defines the
    // IAuthenticationRequestHandler interface, which is derived from the IAuthenticationHandler interface.
    // I'm writing the authentication handler using protected virtual methods
    // so that I can easily create subclasses to work with real authentication services in Chapter 23.
    public class ExternalAuthenticationHandler : IAuthenticationRequestHandler
    {
        private readonly IDataProtectionProvider _dataProtection;
        private readonly ILogger<ExternalAuthenticationHandler> _logger;
        private HttpContext _context;

        public ExternalAuthenticationHandler(
            IOptions<ExternalAuthenticationOptions> options,
            IDataProtectionProvider dataProtection,
            ILogger<ExternalAuthenticationHandler> logger)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _dataProtection = dataProtection ?? throw new ArgumentNullException(nameof(dataProtection));
            _logger = logger;
        }

        public ExternalAuthenticationOptions Options { get; protected set; }
        public PropertiesDataFormat PropertiesFormatter { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.Redirect(await GetAuthenticationUrl(properties));
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        // The HandleRequestAsync method is called automatically by the ASP.NET Core authentication
        // middleware and allows authentication handlers to intercept requests without the need to create custom
        // middleware classes or endpoints.
        public virtual async Task<bool> HandleRequestAsync()
        {
            if (_context.Request.Path.Equals(Options.RedirectPath))
            {
                var authCode = await GetAuthenticationCode();
                (string token, string state) = await GetAccessToken(authCode);

                if (!string.IsNullOrEmpty(token))
                {
                    // TODO: Process token.
                }
                _context.Response.Redirect(string.Format(Options.ErrorUrlTemplate, ErrorMessage));

                return true;
            }
            return false;
        }

        protected virtual Task<string> GetAuthenticationCode()
        {
            return Task.FromResult(_context.Request.Query["code"].ToString());
        }

        protected virtual async Task<(string code, string state)> GetAccessToken(string code)
        {
            var state = (string)_context.Request.Query["state"];
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await httpClient.PostAsJsonAsync(Options.ExchangeUrl,
                new
                {
                    code = code,
                    redirect_uri = Options.RedirectRoot + Options.RedirectPath,
                    client_id = Options.ClientId,
                    client_secret = Options.ClientSecret,
                    state = state,
                    grant_type = "authorization_code",
                });

            var jsonData = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonData);
            var error = jsonDoc.RootElement.GetString("error");

            if (error != null)
            {
                ErrorMessage = "Access Token Error";
                _logger.LogError(ErrorMessage);
                _logger.LogError(jsonData);
            }

            string token = jsonDoc.RootElement.GetString("access_token");
            string jsonState = jsonDoc.RootElement.GetString("state") ?? state;

            return error == null ? (token, state) : (null, null);
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _context = context;

            var purpose = typeof(ExternalAuthenticationOptions).FullName;
            var protector = _dataProtection.CreateProtector(purpose);

            PropertiesFormatter = new PropertiesDataFormat(protector);

            return Task.CompletedTask;
        }

        protected virtual Task<string> GetAuthenticationUrl(AuthenticationProperties properties)
        {
            var queryStringValues = new Dictionary<string, string>
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", Options.RedirectRoot + Options.RedirectPath },
                { "scope", Options.Scope },
                { "response_type", "code" },
                { "state", PropertiesFormatter.Protect(properties) }
            };

            return Task.FromResult(Options.AuthenticationUrl + QueryString.Create(queryStringValues));
        }
    }
}