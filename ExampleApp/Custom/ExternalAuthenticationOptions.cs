namespace ExampleApp.Custom
{
    public class ExternalAuthenticationOptions
    {
        public string ClientId { get; set; } = "MyClientID";
        public string ClientSecret { get; set; } = "MyClientSecret";
        public virtual string RedirectRoot { get; set; } = "https://localhost:44324";
        public virtual string RedirectPath { get; set; } = "/signin-external";
        public virtual string Scope { get; set; } = "openid email profile";
        public virtual string StateHashSecret { get; set; } = "mysecret";
        
        public virtual string AuthenticationUrl { get; set; }
            = "https://localhost:44324/DemoExternalAuthentication/Authenticate";
        
        public virtual string ExchangeUrl { get; set; }
            = "https://localhost:44324/DemoExternalAuthentication/Exchange";

        public virtual string ErrorUrlTemplate { get; set; } = "/ExternalSignin?error={0}";

    }
}