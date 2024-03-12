using HomeBanking.DTOs;
using HomeBanking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoansService _loansService;
        private IUsersService _usersService;

        public LoansController(ILoansService loansService, IUsersService usersService)
        {
            _loansService = loansService;
            _usersService = usersService;
        }


        [HttpGet]
        [Authorize(Roles = "CLIENT, ADMIN")]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<LoanDTO> loansDTOs = _loansService.GetAllLoans();

                return Ok(loansDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "CLIENT, ADMIN")]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                if (loanApplicationDTO.Amount <= 0)
                    return Forbid();

                if (string.IsNullOrEmpty(loanApplicationDTO.Payments))
                    return Forbid();

                if (string.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber))
                    return Forbid();

                string email = _usersService.GetCurrentClientLoggedEmail(User);

                _loansService.CreateLoan(loanApplicationDTO, email);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    } 
}