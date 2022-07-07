namespace ExampleApp.Custom
{
    public class GoogleAuthenticationOptions : ExternalAuthenticationOptions
    {
        public override string RedirectPath => "/signin-google";

        public override string AuthenticationUrl => "https://accounts.google.com/o/oauth2/v2/auth";

        public override string ExchangeUrl => "https://www.googleapis.com/oauth2/v4/token";

        public override string UserInfoUrl => "https://www.googleapis.com/oauth2/v2/userinfo";
    }
}