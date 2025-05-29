using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Table("FDTypes")]
public partial class Fdtype
{
    [Key]
    [Column("FDTypeId")]
    public int FdtypeId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InterestRate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MinAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MaxAmount { get; set; }

    public int Duration { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [InverseProperty("Fdtype")]
    public virtual ICollection<Fdapplication> Fdapplications { get; set; } = new List<Fdapplication>();
}
