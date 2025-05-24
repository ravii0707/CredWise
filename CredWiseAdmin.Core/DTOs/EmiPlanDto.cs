using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public double InterestRate { get; set; }
    }
}
