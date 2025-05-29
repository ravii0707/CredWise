using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class PersonalLoanDetail
{
    [Key]
    public int LoanProductId { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InterestRate { get; set; }

    public int TenureMonths { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ProcessingFee { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MinSalaryRequired { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [ForeignKey("LoanProductId")]
    [InverseProperty("PersonalLoanDetail")]
    public virtual LoanProduct LoanProduct { get; set; } = null!;
}
