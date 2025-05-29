using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class PaymentTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int LoanApplicationId { get; set; }

    public int RepaymentId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(20)]
    public string TransactionStatus { get; set; } = null!;

    [StringLength(100)]
    public string? TransactionReference { get; set; }

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
    [InverseProperty("PaymentTransactions")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;

    [ForeignKey("RepaymentId")]
    [InverseProperty("PaymentTransactions")]
    public virtual LoanRepaymentSchedule Repayment { get; set; } = null!;
}
