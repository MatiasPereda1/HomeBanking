using HomeBanking.DTOs;
using HomeBanking.Models.Enums;
using HomeBanking.Models;
using HomeBanking.Repositories;
using System.Transactions;

namespace HomeBanking.Services
{
    public class TransactionsService : ITransactionsService
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsService(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public void CreateTransaction(TransferDTO transferDTO, string email)
        {
            Client fromClient = _clientRepository.FindByEmail(email);

            var fromAccount = fromClient.Accounts.FirstOrDefault(account => account.Number == transferDTO.FromAccountNumber);

            var toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);

            if (fromClient is null)
                throw new Exception("Cliente remitente no encontrado");

            if (fromAccount is null)
                throw new Exception("Cuenta remitente no encontrado");

            if (toAccount is null)
                throw new Exception("Cuenta destinataria no encontrada");

            if (transferDTO.Amount > fromAccount.Balance)
                throw new Exception("Monto invalido");


            var transactionFrom = new Models.Transaction()
            {
                AccountId = fromAccount.Id,
                Type = TransactionType.DEBIT,
                Amount = transferDTO.Amount * -1,
                Description = transferDTO.Description,
                Date = DateTime.Now
            };

            var transactionTo = new Models.Transaction()
            {
                AccountId = toAccount.Id,
                Type = TransactionType.CREDIT,
                Amount = transferDTO.Amount,
                Description = transferDTO.Description,
                Date = DateTime.Now
            };

            using (var scope = new TransactionScope())
            {
                _transactionRepository.Save(transactionFrom);
                _transactionRepository.Save(transactionTo);

                fromAccount.Balance -= transferDTO.Amount;
                toAccount.Balance += transferDTO.Amount;

                _accountRepository.Save(fromAccount);
                _accountRepository.Save(toAccount);

                scope.Complete();
            }
        }
    }
}