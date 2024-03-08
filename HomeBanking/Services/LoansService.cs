using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories;
using Sqids;
using System.Transactions;

namespace HomeBanking.Services
{
    public class LoansService : ILoansService
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private SqidsEncoder<long> _sqids;

        public LoansService(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, SqidsEncoder<long> sqids)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _sqids = sqids;
        }

        public IEnumerable<LoanDTO> GetAllLoans() 
        {
            IEnumerable<Loan> loans = _loanRepository.GetAllLoans();

            return loans.Select(loan => new LoanDTO(loan, _sqids));
        }

        public void CreateLoan(LoanApplicationDTO loanApplicationDTO, string email)
        {
            var client = _clientRepository.FindByEmail(email);

            var loan = _loanRepository.FindById(_sqids.Decode(loanApplicationDTO.LoanId).FirstOrDefault());

            if (client is null)
                throw new Exception("Cliente no encontrado");

            if (loan == null)
                throw new Exception("Prestamo no encontrado");

            if (loan.Payments.Split(",").Any(payment => payment == loanApplicationDTO.Payments) == false)
                throw new Exception("Cantidad cuotas invalidas");

            if (loan.MaxAmount < loanApplicationDTO.Amount)
                throw new Exception("Monto prestamo invalido");

            var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);

            if (account.ClientId != client.Id)
                throw new Exception("Cuenta invalida");

            var clientLoan = new ClientLoan()
            {
                Amount = Math.Truncate((loanApplicationDTO.Amount * 1.2) * 100) / 100,
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

            using (var scope = new TransactionScope())
            {
                account.Balance += loanApplicationDTO.Amount;
                account.Transactions.Add(transaction);

                _accountRepository.Save(account);
                _clientLoanRepository.Save(clientLoan);

                scope.Complete();
            }
        }
    }
}
