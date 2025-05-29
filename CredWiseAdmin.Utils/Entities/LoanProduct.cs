using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanProduct
{
    [Key]
    public int LoanProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    [StringLength(150)]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MaxLoanAmount { get; set; }

    [StringLength(20)]
    public string LoanType { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [InverseProperty("LoanProduct")]
    public virtual GoldLoanDetail? GoldLoanDetail { get; set; }

    [InverseProperty("LoanProduct")]
    public virtual HomeLoanDetail? HomeLoanDetail { get; set; }

    [InverseProperty("LoanProduct")]
    public virtual ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();

    [InverseProperty("LoanProduct")]
    public virtual ICollection<LoanProductDocument> LoanProductDocuments { get; set; } = new List<LoanProductDocument>();

    [InverseProperty("LoanProduct")]
    public virtual PersonalLoanDetail? PersonalLoanDetail { get; set; }
}
