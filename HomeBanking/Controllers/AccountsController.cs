using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sqids;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        private readonly SqidsEncoder<long> _sqids;

        public AccountsController(IAccountRepository accountRepository, SqidsEncoder<long> sqids)
        {
            _accountRepository = accountRepository;
            _sqids = sqids;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {                
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTOs = new List<AccountDTO>();

                foreach (var account in accounts) 
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = _sqids.Encode(account.Id),
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                        Transactions = account.Transactions.Select(transaction => new TransactionDTO
                        {
                            Type = transaction.Type.ToString(),
                            Amount = transaction.Amount,
                            Description = transaction.Description,
                            Date = transaction.Date,
                        }).ToList()
                    };
                    accountsDTOs.Add(newAccountDTO);
                }
                return Ok(accountsDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get(string id)
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Account account = _accountRepository.FindByIdAndClientEmail(_sqids.Decode(id).FirstOrDefault(), email);

                if (account == null)
                    return Forbid();

                var accountDto = new AccountDTO
                {
                    Id = _sqids.Encode(account.Id),
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(transaction => new TransactionDTO
                    {
                        Type = transaction.Type.ToString(),
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        Date = transaction.Date,
                    }).ToList()
                };
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }        
    }
}
