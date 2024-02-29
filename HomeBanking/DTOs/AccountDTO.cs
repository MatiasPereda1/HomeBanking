using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public IEnumerable<TransactionDTO> Transactions { get; set; }
    }
}
