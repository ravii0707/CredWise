using System;
using System.ComponentModel.DataAnnotations;

namespace CredWiseAdmin.Core.DTOs
{
    public class EmiPlanDto
    {
        [Required]
        public int LoanId { get; set; }

        [Required]
        [Range(1, 24, ErrorMessage = "EMI plan must be between 6, 12, or 24 months")]
        public int TenureInMonths { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Interest rate must be a positive percentage")]
        public decimal InterestRate { get; set; }

        public decimal LoanAmount { get; set; }
        public DateTime StartDate { get; set; }
    }
}
