using HomeBanking.Models;

namespace HomeBanking.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();
        Loan FindById(long id);
    }
}
