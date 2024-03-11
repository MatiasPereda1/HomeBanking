using HomeBanking.Models;
using Sqids;
using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class ClientDTO
    {
        public ClientDTO(Client client, SqidsEncoder<long> sqids)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Role = client.Role.ToString();
            Accounts = client.Accounts.Select(account => new AccountDTO(account, sqids));
            Loans = client.ClientLoans.Select(clientLoan => new ClientLoanDTO(clientLoan));
            Cards = client.Cards.Select(card => new CardOutDTO(card));
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public IEnumerable<AccountDTO> Accounts { get; set; }
        public IEnumerable<ClientLoanDTO> Loans { get; set; }
        public IEnumerable<CardOutDTO> Cards { get; set; }
    }
}