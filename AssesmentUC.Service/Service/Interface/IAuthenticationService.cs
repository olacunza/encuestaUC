using System.Security.Claims;

namespace AssesmentUC.Service.Service.Interface
{

    public interface IAuthenticationService
    {
        string? GetUserEmail(ClaimsPrincipal principal);
        string? GetUserId(ClaimsPrincipal principal);
        string? GetUserName(ClaimsPrincipal principal);
        string? GetUserPicture(ClaimsPrincipal principal);
        bool IsEmailVerified(ClaimsPrincipal principal);
        IEnumerable<Claim> GetAllClaims(ClaimsPrincipal principal);
    }
}
