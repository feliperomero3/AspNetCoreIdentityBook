namespace ExampleApp.Custom
{
    public class FacebookAuthenticationOptions : ExternalAuthenticationOptions
    {
        public override string RedirectPath { get; set; } = "/signin-facebook";
        public override string Scope { get; set; } = "email";
        public override string AuthenticationUrl => "https://www.facebook.com/v8.0/dialog/oauth";
        public override string ExchangeUrl { get; set; } = "https://graph.facebook.com/v8.0/oauth/access_token";
        public override string UserInfoUrl { get; set; } = "https://graph.facebook.com/v8.0/me?fields=name,email";
    }
}