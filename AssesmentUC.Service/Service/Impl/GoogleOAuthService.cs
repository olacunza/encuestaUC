using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Configuration;

namespace AssesmentUC.Service.Service.Impl
{
    public class GoogleOAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleOAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var clientId = _configuration["SmtpSettings:ClientId"];
            var clientSecret = _configuration["SmtpSettings:ClientSecret"];
            var refreshToken = _configuration["SmtpSettings:RefreshToken"];

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            });

            var token = new TokenResponse
            {
                RefreshToken = refreshToken
            };

            var credential = new UserCredential(flow, "user", token);

            // Si el access token expira lo renueva automáticamente
            if (await credential.RefreshTokenAsync(CancellationToken.None))
            {
                return credential.Token.AccessToken;
            }

            throw new Exception("No se pudo renovar el Access Token");
        }
    }
}
