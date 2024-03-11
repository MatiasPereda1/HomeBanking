using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBanking.DTOs;
using HomeBanking.Utils;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private readonly IConfiguration _config;

        public AuthController(IClientRepository clientRepository, IConfiguration config)
        {
            _clientRepository = clientRepository;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] ClientInDTO userDto)
        {
            try
            {
                Client client = _clientRepository.FindByEmail(userDto.Email);

                if (client is null || !PasswordsUtils.VerifyPassword(userDto.Password, client.Password))
                    throw new Exception("Contraseña o usuario incorrectos");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new List<Claim>
                {         
                    new Claim(ClaimTypes.Email, client.Email),
                    new Claim(ClaimTypes.GivenName, client.FirstName),
                    new Claim(ClaimTypes.Surname, client.LastName),
                    new Claim(ClaimTypes.Role, client.Role.ToString())
                };

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));       
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
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}