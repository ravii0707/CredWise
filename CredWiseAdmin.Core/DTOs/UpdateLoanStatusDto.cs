using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class UpdateLoanStatusDto
    {
        [Required]
        public required string Status { get; set; } // Approved, Rejected

        [Required]
        public required string Reason { get; set; }
    }
}
