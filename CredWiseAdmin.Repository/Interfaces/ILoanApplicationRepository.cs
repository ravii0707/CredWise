using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface ILoanApplicationRepository
    {
        Task<IEnumerable<LoanApplication>> GetAllAsync();
        Task<LoanApplication> GetByIdAsync(int id);
        Task<IEnumerable<LoanApplication>> GetByUserIdAsync(int userId);
        Task AddAsync(LoanApplication application);
        Task UpdateAsync(LoanApplication application);
        Task<bool> HasActiveLoan(int userId);
        Task<IEnumerable<LoanApplication>> GetPendingApplicationsAsync();
        Task<IEnumerable<LoanApplication>> GetApprovedApplicationsAsync();
        Task<bool> IsAadhaarUsed(string aadhaar);
        Task<IEnumerable<LoanApplication>> GetAllLoanApplicationsAsync();
    }
}
