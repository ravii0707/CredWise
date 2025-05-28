using CredWiseAdmin.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface ILoanRepaymentRepository
    {
        Task<IEnumerable<LoanRepaymentSchedule>> GetByLoanApplicationIdAsync(int loanApplicationId);
        Task<LoanRepaymentSchedule> GetByIdAsync(int id);
        Task AddAsync(LoanRepaymentSchedule repayment);
        Task UpdateAsync(LoanRepaymentSchedule repayment);
        Task<IEnumerable<LoanRepaymentSchedule>> GetPendingRepaymentsAsync(int userId);
        Task<IEnumerable<LoanRepaymentSchedule>> GetOverdueRepaymentsAsync();
        Task<IEnumerable<LoanRepaymentSchedule>> GetAllRepaymentsAsync();
    }
}