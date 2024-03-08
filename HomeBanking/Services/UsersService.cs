using HomeBanking.Models;
using System.Security.Claims;

namespace HomeBanking.Services
{
    public class UsersService : IUsersService
    {

        public string GetCurrentClientLoggedEmail(ClaimsPrincipal user)
        {
            if (user.FindFirst("Client") == null && user.FindFirst("Admin") == null)
                throw new Exception("Usuario no loggeado");

            return user.FindFirst("Client") == null ? user.FindFirst("Admin").Value : user.FindFirst("Client").Value;
        }
    }
}
