using HomeBanking.Models;

namespace HomeBanking.Repositories
{
    public interface ICardRepository
    {
        void Save(Card card);
        bool ExistsCardNumber(string cardNumber);
    }
}


