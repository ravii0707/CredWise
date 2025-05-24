using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Data.Repositories.Interfaces
{
    public interface ILoanBankStatementRepository
    {
        Task<LoanBankStatement> GetByIdAsync(int id);
        Task<IEnumerable<LoanBankStatement>> GetByLoanApplicationIdAsync(int loanApplicationId);
        Task AddAsync(LoanBankStatement statement);
        Task UpdateAsync(LoanBankStatement statement);
        Task DeleteAsync(int id);
    }
}
