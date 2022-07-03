namespace ExampleApp.Custom
{
    public class ExternalAuthenticationOptions
    {
        public string ClientId { get; set; } = "MyClientID";
        public string ClientSecret { get; set; } = "MyClientSecret";

        // I'm writing the authentication handler using protected virtual methods
        // so that I can easily create subclasses to work with real authentication services in Chapter 23.
        public virtual string RedirectRoot { get; set; } = "https://localhost:44324";
        public virtual string RedirectPath { get; set; } = "/signin-external";
        public virtual string Scope { get; set; } = "openid email profile";
        public virtual string StateHashSecret { get; set; } = "mysecret";
        public virtual string AuthenticationUrl { get; set; }
            = "https://localhost:44324/DemoExternalAuthentication/Authenticate";
    }
}