using CredWiseAdmin.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface ILoanApplicationService
    {
        Task<LoanApplicationResponseDto> CreateLoanApplicationAsync(LoanApplicationDto applicationDto);
        Task<LoanApplicationResponseDto> GetLoanApplicationByIdAsync(int id);
        Task<IEnumerable<LoanApplicationResponseDto>> GetLoanApplicationsByUserIdAsync(int userId);
        Task<IEnumerable<LoanApplicationResponseDto>> GetPendingLoanApplicationsAsync();
        Task<IEnumerable<LoanApplicationResponseDto>> GetApprovedLoanApplicationsAsync();
        Task<bool> UploadBankStatementAsync(int loanApplicationId, UploadBankStatementDto uploadDto);
        Task<bool> SendToDecisionAppAsync(int loanApplicationId);
        Task<bool> UpdateLoanStatusAsync(int loanApplicationId, string status, string reason);
        Task<bool> FinalizeRepaymentAsync(int loanApplicationId);
        Task<RepaymentPlanResponseDto> GenerateRepaymentPlanAsync(EmiPlanDto emiPlanDto);
        Task<IEnumerable<LoanApplicationResponseDto>> GetAllLoanApplicationsAsync();
    }
}
