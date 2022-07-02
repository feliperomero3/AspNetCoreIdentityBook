using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private static readonly string _expectedID = "MyClientID";
        private static readonly string _expectedSecret = "MyClientSecret";

        private static readonly List<UserInfo> _users = new()
        {
            new UserInfo
            {
                Id = "1",
                Name = "Alice",
                EmailAddress = "alice@example.com",
                Password = "myexternalpassword"
            },
            new UserInfo
            {
                Id = "2",
                Name = "Dora",
                EmailAddress = "dora@example.com",
                Password = "myexternalpassword"
            }
        };
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
    }
}