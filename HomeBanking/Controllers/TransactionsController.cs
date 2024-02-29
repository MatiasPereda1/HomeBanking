using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(transferDTO.FromAccountNumber))
                    return StatusCode(403, "cuenta del remitente inválida");

                if (string.IsNullOrEmpty(transferDTO.ToAccountNumber))
                    return StatusCode(403, "cuenta del destinatario inválida");

                if (string.IsNullOrEmpty(transferDTO.Description))
                    return StatusCode(403, "descripcion inválido");

                if (transferDTO.Amount <= 0)
                    return StatusCode(403, "monto inválido");

                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                    return Forbid();


                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client fromClient = _clientRepository.FindByEmail(email);            

                var fromAccount = fromClient.Accounts.FirstOrDefault(account => account.Number == transferDTO.FromAccountNumber);

                var toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);

                if (fromClient == null)
                {
                    return Forbid();
                }                

                if (fromAccount is null)
                    return Forbid();

                if (toAccount is null)
                    return Forbid();

                if (transferDTO.Amount > fromAccount.Balance)
                    return Forbid();


                var transactionFrom = new Transaction()
                {
                    AccountId = fromAccount.Id,
                    Type = TransactionType.DEBIT,
                    Amount = transferDTO.Amount * -1,
                    Description = transferDTO.Description,
                    Date = DateTime.Now
                };

                var transactionTo = new Transaction()
                {
                    AccountId = toAccount.Id,
                    Type = TransactionType.CREDIT,
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description,
                    Date = DateTime.Now
                };

                _transactionRepository.Save(transactionFrom);
                _transactionRepository.Save(transactionTo);

                fromAccount.Balance -= transferDTO.Amount;
                toAccount.Balance += transferDTO.Amount;

                _accountRepository.Save(fromAccount);
                _accountRepository.Save(toAccount);

                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
