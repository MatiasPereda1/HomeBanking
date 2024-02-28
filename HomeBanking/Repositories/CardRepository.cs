using HomeBanking.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBanking.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public bool ExistsCardNumber(string cardNumber)
        {
            return GetAllCards().Any(card => card.Number == cardNumber);
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll()
                .Include(card => card.Client)
                .ToList();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
