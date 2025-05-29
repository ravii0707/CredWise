using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class LoanApplicationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string? Aadhaar { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public decimal Income { get; set; }

        [Required]
        public string? EmploymentType { get; set; } // Self-Employed, Salaried

        [Required]
        public int LoanProductId { get; set; }

        [Required]
        public decimal RequestedAmount { get; set; }

        [Required]
        public int RequestedTenure { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        //public string? Purpose { get; set; }
    }

    public class LoanApplicationResponseDto : LoanApplicationDto
    {
        public int LoanApplicationId { get; set; }
        public string? Status { get; set; }
        public DateTime DecisionDate { get; set; }
        public string? DecisionReason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LoanType { get; set; } // From LoanProduct
    }
}
