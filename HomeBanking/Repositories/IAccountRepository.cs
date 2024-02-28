using HomeBanking.Models;

namespace HomeBanking.Repositories
{
    public interface IAccountRepository 
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FindByIdAndClientEmail(long id, string email);
        bool ExistsAccountNumber(string accountNumber);
    }
}
