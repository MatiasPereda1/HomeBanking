using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using HomeBanking.DTOs;
using HomeBanking.Utils;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            try
            {
                Client client = _clientRepository.FindByEmail(userDto.Email);
                if (client == null || !PasswordsUtils.VerifyPassword(userDto.Password, client.Password))
                    return Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(Regex.IsMatch(userDto.Email, @".*@vinotinto\.com") ? "Admin" : "Client", client.Email)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

