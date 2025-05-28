using CredWiseAdmin.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface ILoanEnquiryRepository
    {
        Task<IEnumerable<LoanEnquiry>> GetAllAsync();
        Task<LoanEnquiry> GetByIdAsync(int id);
    }
} 