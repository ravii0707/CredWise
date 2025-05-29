using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class HomeLoanApplication
{
    [Key]
    public int HomeLoanAppId { get; set; }

    public int LoanApplicationId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string PropertyAddress { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DownPaymentPercentage { get; set; }

    [ForeignKey("LoanApplicationId")]
    [InverseProperty("HomeLoanApplications")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;
}
