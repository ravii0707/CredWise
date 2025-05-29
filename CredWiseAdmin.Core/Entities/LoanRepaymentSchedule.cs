using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Table("LoanRepaymentSchedule")]
public partial class LoanRepaymentSchedule
{
    [Key]
    public int RepaymentId { get; set; }

    public int LoanApplicationId { get; set; }

    public int InstallmentNumber { get; set; }

    public DateOnly DueDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrincipalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal InterestAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

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
    [InverseProperty("LoanRepaymentSchedules")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;

    [InverseProperty("Repayment")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
