using HomeBanking.DTOs;

namespace HomeBanking.Services
{
    public interface ITransactionsService
    {
        public void CreateTransaction(TransferDTO transferDTO, string email);
    }
}
