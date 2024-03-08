using HomeBanking.Models;
using Sqids;
using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class AccountDTO
    {
        public AccountDTO(Account account, SqidsEncoder<long> sqids)
        {
            Id = sqids.Encode(account.Id);
            Balance = account.Balance;
            CreationDate = account.CreationDate;
            Number = account.Number;
        }

        public string Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public IEnumerable<TransactionDTO> Transactions { get; set; }
    }
}