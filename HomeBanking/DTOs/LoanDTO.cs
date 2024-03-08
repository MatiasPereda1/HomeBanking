using HomeBanking.Models;
using Sqids;

namespace HomeBanking.DTOs
{
    public class LoanDTO
    {
        public LoanDTO(Loan loan, SqidsEncoder<long> sqids)
        {
            Id = sqids.Encode(loan.Id);
            Name = loan.Name;
            MaxAmount = loan.MaxAmount;
            Payments = loan.Payments;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public double MaxAmount { get; set; }
        public string Payments { get; set; }
    }
}