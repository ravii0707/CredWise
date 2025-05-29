using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class LoanDocumentUploadDto
{
    [Required]
    public int LoanApplicationId { get; set; }

    [Required]
    public string DocumentType { get; set; } // e.g., "Aadhaar", "PAN", "BankStatement"

    [Required]
    public IFormFile Document { get; set; }

    public string? Description { get; set; }
}
