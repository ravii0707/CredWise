using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class GoldLoanApplication
{
    [Key]
    public int GoldLoanAppId { get; set; }

    public int LoanApplicationId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal GoldWeight { get; set; }

    [StringLength(10)]
    public string GoldPurity { get; set; } = null!;

    [ForeignKey("LoanApplicationId")]
    [InverseProperty("GoldLoanApplications")]
    public virtual LoanApplication LoanApplication { get; set; } = null!;
}
