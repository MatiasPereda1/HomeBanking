using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories;
using Sqids;

namespace HomeBanking.Services
{
    public class AccountsService : IAccountsService
    {
        private IAccountRepository _accountRepository;
        private readonly SqidsEncoder<long> _sqids;

        public AccountsService(IAccountRepository accountRepository, SqidsEncoder<long> sqids)
        {
            _accountRepository = accountRepository;
            _sqids = sqids;
        }

        public IEnumerable<AccountDTO> GetAllAccounts() 
        {
            IEnumerable<Account> accounts = _accountRepository.GetAllAccounts();

            return accounts.Select(account => new AccountDTO(account, _sqids));
        }

        public AccountDTO GetAccount(string id, string clientEmail)
        {
            Account account = _accountRepository.FindByIdAndClientEmail(_sqids.Decode(id).FirstOrDefault(), clientEmail);

            if (account is null)
                throw new Exception("Cuenta no encontrada");

            AccountDTO accountDTO = new AccountDTO(account, _sqids);

            accountDTO.Transactions = account.Transactions.Select(transaction => new TransactionDTO(transaction));

            return accountDTO;
        }
    }
}