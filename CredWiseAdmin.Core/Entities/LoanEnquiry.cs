using System;
using System.ComponentModel.DataAnnotations;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanEnquiry
{
    [Key]
    public int EnquiryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(15)]
    public string PhoneNumber { get; set; }

    public decimal LoanAmountRequired { get; set; }

    [Required]
    public string LoanPurpose { get; set; }

    public DateTime CreatedAt { get; set; }
}
