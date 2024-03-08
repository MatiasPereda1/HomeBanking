using HomeBanking.DTOs;
using HomeBanking.Models;

namespace HomeBanking.Services
{
    public interface IClientsService
    {
        public IEnumerable<ClientDTO> GetAllClients();
        public ClientDTO GetClientById(long id);
        public ClientDTO GetClientByEmail(string email);
        public Client CreateClient(ClientInDTO userDTO);
        public AccountDTO CreateAccount(string email);
        public CardOutDTO CreateCard(string email, CardInDTO cardInDTO);
        public IEnumerable<AccountDTO> GetAccounts(string email);
        public IEnumerable<CardOutDTO> GetCards(string email);
    }
}
