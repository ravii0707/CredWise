using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanEnquiry
{
    [Key]
    public int EnquiryId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LoanAmountRequired { get; set; }

    [StringLength(255)]
    public string LoanPurpose { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
