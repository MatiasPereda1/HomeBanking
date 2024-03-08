using System.Security.Claims;

namespace HomeBanking.Services
{
    public interface IUsersService
    {
        public string GetCurrentClientLoggedEmail(ClaimsPrincipal user);
    }
}
