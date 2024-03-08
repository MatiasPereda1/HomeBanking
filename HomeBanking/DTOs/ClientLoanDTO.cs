using HomeBanking.Models;
using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class ClientLoanDTO
    {
        public ClientLoanDTO(ClientLoan clientLoan)
        {
            LoanId = clientLoan.LoanId;
            Name = clientLoan.Loan.Name;
            Amount = clientLoan.Amount;
            Payments = int.Parse(clientLoan.Payments); 
        }

        public long LoanId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Payments { get; set; }
    }
}