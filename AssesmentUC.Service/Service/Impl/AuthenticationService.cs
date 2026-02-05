using AssesmentUC.Service.Service.Interface;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace AssesmentUC.Service.Service.Impl
{

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            _logger = logger;
        }

        public string? GetUserEmail(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al obtener email");
                return null;
            }

            var email = principal.FindFirst("email")?.Value ?? principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                _logger.LogWarning("Email no encontrado en los claims");
            
            return email;
        }

        public string? GetUserId(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al obtener userId");
                return null;
            }

            var userId = principal.FindFirst("sub")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                _logger.LogWarning("ID de usuario (sub) no encontrado en los claims");
            
            return userId;
        }

        public string? GetUserName(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al obtener nombre");
                return null;
            }

            var name = principal.FindFirst("name")?.Value ?? principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(name))
                _logger.LogWarning("Nombre de usuario no encontrado en los claims");
            
            return name;
        }

        public string? GetUserPicture(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al obtener picture");
                return null;
            }

            var picture = principal.FindFirst("picture")?.Value;

            return picture;
        }

        public bool IsEmailVerified(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al verificar email");
                return false;
            }

            var emailVerified = principal.FindFirst("email_verified")?.Value;

            if (bool.TryParse(emailVerified, out var isVerified))
                return isVerified;

            return false;
        }

        public IEnumerable<Claim> GetAllClaims(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                _logger.LogWarning("ClaimsPrincipal es nulo al obtener claims");
                return Enumerable.Empty<Claim>();
            }

            return principal.Claims;
        }
    }
}
