using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Table("FDApplications")]
public partial class Fdapplication
{
    [Key]
    [Column("FDApplicationId")]
    public int FdapplicationId { get; set; }

    public int UserId { get; set; }

    [Column("FDTypeId")]
    public int FdtypeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public int Duration { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InterestRate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? MaturityDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MaturityAmount { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [InverseProperty("Fdapplication")]
    public virtual ICollection<Fdtransaction> Fdtransactions { get; set; } = new List<Fdtransaction>();

    [ForeignKey("FdtypeId")]
    [InverseProperty("Fdapplications")]
    public virtual Fdtype Fdtype { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Fdapplications")]
    public virtual User User { get; set; } = null!;
}
