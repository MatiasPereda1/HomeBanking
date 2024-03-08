using HomeBanking.DTOs;
using System.Security.Claims;

namespace HomeBanking.Services
{
    public interface IAuthService
    {
        public ClaimsIdentity Login(ClientInDTO userDto);
    }
}
