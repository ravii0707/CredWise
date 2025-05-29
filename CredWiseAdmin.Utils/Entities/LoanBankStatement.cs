using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanBankStatement
{
    [Key]
    public int BankStatementId { get; set; }

    public int LoanApplicationId { get; set; }

    [StringLength(100)]
    public string DocumentName { get; set; } = null!;

    [StringLength(500)]
    public string DocumentPath { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(255)]
    public string? RejectionReason { get; set; }

    public int? VerifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? VerifiedAt { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [ForeignKey("LoanApplicationId")]
    [InverseProperty("LoanBankStatements")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;

    [ForeignKey("VerifiedBy")]
    [InverseProperty("LoanBankStatements")]
    public virtual User? VerifiedByNavigation { get; set; }
}
