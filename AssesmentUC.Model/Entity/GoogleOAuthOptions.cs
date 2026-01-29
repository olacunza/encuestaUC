namespace AssesmentUC.Model.Entity
{

    public class GoogleOAuthOptions
    {

        public const string SectionName = "GoogleOAuth";

        public string Authority { get; set; } = "https://accounts.google.com";

        public string ClientId { get; set; } = string.Empty;

        public string ValidIssuer { get; set; } = "https://accounts.google.com";

        public bool ValidateIssuer { get; set; } = true;

        public bool ValidateAudience { get; set; } = true;

        public bool ValidateLifetime { get; set; } = true;
    }
}
