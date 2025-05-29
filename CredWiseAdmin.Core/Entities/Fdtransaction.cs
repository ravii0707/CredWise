using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Table("FDTransactions")]
public partial class Fdtransaction
{
    [Key]
    [Column("FDTransactionId")]
    public int FdtransactionId { get; set; }

    [Column("FDApplicationId")]
    public int FdapplicationId { get; set; }

    [StringLength(20)]
    public string TransactionType { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransactionDate { get; set; }

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

    [ForeignKey("FdapplicationId")]
    [InverseProperty("Fdtransactions")]
    public virtual Fdapplication Fdapplication { get; set; } = null!;
}
