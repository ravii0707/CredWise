using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface ILoanProductRepository
    {
        Task<IEnumerable<LoanProduct>> GetAllAsync();
        Task<LoanProduct> GetByIdAsync(int id);
        Task<IEnumerable<LoanProduct>> GetActiveProductsAsync();
        Task AddAsync(LoanProduct product);
        Task UpdateAsync(LoanProduct product);
        Task DeleteAsync(int id);
    }
}
