using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class LoanProductDto
    {
        [Required]
        public string? ImageUrl { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaxLoanAmount { get; set; }

        [Required]
        public string? LoanType { get; set; } // HOME, PERSONAL, GOLD
    }

    
}
