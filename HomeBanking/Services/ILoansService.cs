using HomeBanking.DTOs;

namespace HomeBanking.Services
{
    public interface ILoansService
    {
        public IEnumerable<LoanDTO> GetAllLoans();
        public void CreateLoan(LoanApplicationDTO loanApplicationDTO, string email);
    }
}
