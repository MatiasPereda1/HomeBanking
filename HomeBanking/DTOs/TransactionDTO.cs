using HomeBanking.Models;
using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class TransactionDTO
    {
        public TransactionDTO(Transaction transaction)
        {
            Type = transaction.Type.ToString();
            Amount = transaction.Amount;
            Description = transaction.Description;
            Date = transaction.Date;
        }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}