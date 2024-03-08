using HomeBanking.DTOs;

namespace HomeBanking.Services
{
    public interface IAccountsService
    {
        IEnumerable<AccountDTO> GetAllAccounts();
        AccountDTO GetAccount(string id, string clientEmail);
    }
}
