using HomeBanking.DTOs;
using HomeBanking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ITransactionsService _transactionsService;
        private IUsersService _usersService;

        public TransactionsController(ITransactionsService transactionsService, IUsersService usersService)
        {
            _transactionsService = transactionsService;
            _usersService = usersService;
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(transferDTO.FromAccountNumber))
                    return StatusCode(403, "Cuenta remitente invalida");

                if (string.IsNullOrEmpty(transferDTO.ToAccountNumber))
                    return StatusCode(403, "Cuenta destinatario invalida");

                if (string.IsNullOrEmpty(transferDTO.Description))
                    return StatusCode(403, "Descripcion inválido");

                if (transferDTO.Amount <= 0)
                    return StatusCode(403, "Monto invalido");

                string email = _usersService.GetCurrentClientLoggedEmail(User);

                _transactionsService.CreateTransaction(transferDTO, email);

                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}