using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers
{
    // For simplicity, I am going to use a controller in the existing ExampleApp project
    // to represent the external authentication service.
    // This will allow me to demonstrate the use of HTTP requests in the external
    // authentication process without needing to create and run a separate server.
    public class DemoExternalAuthenticationController : Controller
    {
        // The controller defines fields that specify the expected values for the client ID and client secret.
        // In a real external service, each application that has been registered will have an ID and secret,
        // but I need only one set of values to demonstrate the authentication sequence in the ExampleApp project.
        private static readonly string _expectedId = "MyClientID";
        private static readonly string _expectedSecret = "MyClientSecret";

        private static readonly List<UserInfo> _users = new()
        {
            new UserInfo
            {
                Id = "1",
                Name = "Alice",
                EmailAddress = "alice@example.com",
                Password = "myexternalpassword",
                Code = "123456",
                Token = "token1"
            },
            new UserInfo
            {
                Id = "2",
                Name = "Dora",
                EmailAddress = "dora@example.com",
                Password = "myexternalpassword",
                Code = "56789",
                Token = "token2"
            }
        };

        public class UserInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string Code { get; set; }
            public string Token { get; set; }
        }

        // Action method that simulates an external service.
        // This action will be the target of the redirection.
        public ActionResult Authenticate([FromQuery] ExternalAuthententicationInfo info)
        {
            return (_expectedId == info.client_id)
                ? View((info, string.Empty))
                : View((info, "Unknown Client"));
        }

        // Action method to receive the credentials provided by the user and validate them.
        // The ASP.NET Core Identity application doesn’t participate in the external authentication process,
        // which is conducted privately between the user and the external authentication service.
        [HttpPost]
        public ActionResult Authenticate(ExternalAuthententicationInfo info, string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password required");
            }
            else
            {
                var user = _users.FirstOrDefault(u => u.EmailAddress.Equals(email) && u.Password.Equals(password));
                if (user is not null)
                {
                    // User has been successfully authenticated.

                    // The URL to which the browser is redirected is determined using the redirect_uri, scope, and state
                    // values provided by the authentication handler in step 1.
                    // The query string also includes a code value, which I have defined statically for each user.
                    // In a real authentication service, the code values are generated dynamically.
                    var url = info.redirect_uri
                        + $"?code={user.Code}"
                        + $"&scope={info.scope}"
                        + $"&state={info.state}";

                    return base.LocalRedirect(url);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                }
            }
            return View((info, string.Empty));
        }

        // The action method locates the user with the specified code and returns the user’s token. In a real
        // authentication service, the tokens are generated dynamically, which means you cannot rely on always
        // receiving the same token for a given user.
        [HttpPost]
        public ActionResult Exchange([FromBody] ExternalAuthententicationInfo info)
        {
            if (info.client_id != _expectedId || info.client_secret != _expectedSecret)
            {
                return UnprocessableEntity(new { error = "unauthorized_client" });
            }

            var user = _users.FirstOrDefault(u => u.Code.Equals(info.code));

            if (user is not null)
            {
                return Ok(new
                {
                    access_token = user.Token,
                    expires_in = 3600,
                    token_type = "Bearer",
                    scope = info.scope,
                    state = info.state
                });
            }
            return UnprocessableEntity(new { error = "Invalid authorization code." });
        }

        // No other information needs to be included in the request because the authentication service
        // can use the tokens it issues to determine which user and application a token relates to.
        // The data that the application receives depends on the authentication service and the scope that has been requested.
        // The data that this example controller produces is simpler but will be sufficient for this chapter.
        [HttpGet("Userinfo")]
        public ActionResult GetUserinfo([FromHeader] string authorization)
        {
            string token = authorization?[7..];
            var user = _users.FirstOrDefault(user => user.Token.Equals(token));
            if (user is not null)
            {
                return Json(new { user.Id, user.EmailAddress, user.Name });
            }
            else
            {
                return Json(new { error = "invalid_token" });
            }
        }
    }

    public class ExternalAuthententicationInfo
    {
#pragma warning disable IDE1006 // Naming Styles
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public string scope { get; set; }
        public string state { get; set; }
        public string response_type { get; set; }
        public string grant_type { get; set; }
        public string code { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}