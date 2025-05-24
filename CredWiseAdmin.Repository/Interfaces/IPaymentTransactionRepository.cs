using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<IEnumerable<PaymentTransaction>> GetByLoanApplicationIdAsync(int loanApplicationId);
        Task<PaymentTransaction> GetByIdAsync(int id);
        Task AddAsync(PaymentTransaction transaction);
        Task UpdateAsync(PaymentTransaction transaction);
    }
}
