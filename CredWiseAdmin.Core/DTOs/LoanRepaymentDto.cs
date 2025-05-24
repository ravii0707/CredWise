using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class LoanRepaymentDto
    {
        public int RepaymentId { get; set; }
        public int LoanApplicationId { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentTransactionDto
    {
        [Required]
        public int LoanApplicationId { get; set; }

        [Required]
        public int RepaymentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }
    }

    public class PaymentTransactionResponseDto : PaymentTransactionDto
    {
        public int TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionReference { get; set; }
    }
}
