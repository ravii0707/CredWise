using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class UploadBankStatementDto
    {
        [Required]
        public int LoanApplicationId { get; set; }

        [Required]
        public IFormFile? Document { get; set; }

        [Required]
        public string? DocumentName { get; set; }
    }

    public class BankStatementResponseDto
    {
        public int BankStatementId { get; set; }
        public int LoanApplicationId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentPath { get; set; }
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime VerifiedAt { get; set; }
        public int VerifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
