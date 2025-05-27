using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendUserRegistrationEmailAsync(string recipientEmail, string password);
        Task SendLoanApprovalEmailAsync(string email, int loanApplicationId);
        Task SendLoanRejectionEmailAsync(string email, string reason);
        Task SendPaymentConfirmationEmailAsync(string email, int transactionId);
    }
}
