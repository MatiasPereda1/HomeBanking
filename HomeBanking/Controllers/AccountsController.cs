using HomeBanking.DTOs;
using HomeBanking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountsService _accountsService;
        private IUsersService _usersService;

        public AccountsController(IAccountsService accountsService, IUsersService usersService)
        {
            _accountsService = accountsService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<AccountDTO> accountsDTOs = _accountsService.GetAllAccounts();
                                
                return Ok(accountsDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CLIENT, ADMIN")]
        public IActionResult Get(string id)
        {
            try
            {
                string clientEmail = _usersService.GetCurrentClientLoggedEmail(User);

                AccountDTO accountDto = _accountsService.GetAccount(id, clientEmail);

                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }        
    }
}