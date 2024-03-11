using HomeBanking.Models;
using System.Security.Claims;

namespace HomeBanking.Services
{
    public class UsersService : IUsersService
    {

        public string GetCurrentClientLoggedEmail(ClaimsPrincipal user)
        {
            var userClaims = user.Claims;

            return userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
        }
    }
}
