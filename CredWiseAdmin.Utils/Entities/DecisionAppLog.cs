using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class DecisionAppLog
{
    [Key]
    public int LogId { get; set; }

    public int LoanApplicationId { get; set; }

    public string? DecisionInput { get; set; }

    public string? DecisionOutput { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProcessedAt { get; set; }

    public int? ProcessingTime { get; set; }

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
    [InverseProperty("DecisionAppLogs")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;
}
