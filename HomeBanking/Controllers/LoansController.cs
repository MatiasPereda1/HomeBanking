using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sqids;
using System.Transactions;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {        
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private SqidsEncoder<long> _sqids;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository,IClientLoanRepository clientLoanRepository, SqidsEncoder<long> sqids)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _sqids = sqids;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();

                var loansDTOs = loans.Select(loan => new LoanDTO()
                {
                    Id = _sqids.Encode(loan.Id),
                    Name = loan.Name,
                    MaxAmount = loan.MaxAmount,
                    Payments = loan.Payments
                }).ToList();

                return Ok(loansDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
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

                var email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                var client = _clientRepository.FindByEmail(email);

                var loan = _loanRepository.FindById( _sqids.Decode(loanApplicationDTO.LoanId).FirstOrDefault() );

                if (loan == null)
                    return Forbid();

                if (loan.Payments.Split(",").Any(payment => payment == loanApplicationDTO.Payments) == false)
                    return Forbid();

                if (loan.MaxAmount < loanApplicationDTO.Amount)
                    return Forbid();

                var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);

                if (account.ClientId != client.Id)
                    return Forbid();

                var clientLoan = new ClientLoan()
                {
                    Amount = Math.Truncate( (loanApplicationDTO.Amount * 1.2) * 100) / 100,
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId = _sqids.Decode(loanApplicationDTO.LoanId).FirstOrDefault()
                };

                var transaction = new Models.Transaction()
                {
                    Type = TransactionType.CREDIT,
                    Amount = loanApplicationDTO.Amount,
                    Description = $"{loan.Name} loan approved",
                    Date = DateTime.Now,
                    AccountId = account.Id
                };

                using(var scope = new TransactionScope())
                {
                    account.Balance += loanApplicationDTO.Amount;
                    account.Transactions.Add(transaction);

                    _accountRepository.Save(account);
                    _clientLoanRepository.Save(clientLoan);

                    scope.Complete();
                }               

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    } 
}
