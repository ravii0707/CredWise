using CredWiseAdmin.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface ILoanRepaymentService
    {
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanIdAsync(int loanApplicationId);
        Task<PaymentTransactionResponseDto> ProcessPaymentAsync(PaymentTransactionDto paymentDto);
        Task<bool> ApplyPenaltyAsync(int repaymentId);
        Task<IEnumerable<LoanRepaymentDto>> GetPendingRepaymentsAsync(int userId);
        Task<IEnumerable<LoanRepaymentDto>> GetOverdueRepaymentsAsync();
        Task<IEnumerable<LoanRepaymentDto>> GetAllRepaymentsAsync();
    }
}
