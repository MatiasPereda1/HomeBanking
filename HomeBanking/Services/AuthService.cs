using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories;
using HomeBanking.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace HomeBanking.Services
{
    public class AuthService : IAuthService
    {
        private IClientRepository _clientRepository;
        public AuthService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public ClaimsIdentity Login(ClientInDTO userDto)
        {
            Client client = _clientRepository.FindByEmail(userDto.Email);
            if (client is null || !PasswordsUtils.VerifyPassword(userDto.Password, client.Password))
                throw new Exception("Contraseña o usuario incorrectos");

            List<Claim> claims = new List<Claim>
                {
                    new Claim(Regex.IsMatch(userDto.Email, @".*@vinotinto\.com") ? "Admin" : "Client", client.Email)
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
                );            

            return claimsIdentity;
        }
    }
}
