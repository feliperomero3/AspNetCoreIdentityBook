namespace ExampleApp.Custom
{
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