using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class LoanEnquiryDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public decimal LoanAmountRequired { get; set; }

        [Required]
        public string LoanPurpose { get; set; }
    }

    public class LoanEnquiryResponseDto : LoanEnquiryDto
    {
        public int EnquiryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
