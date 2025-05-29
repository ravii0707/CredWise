using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Index("Aadhaar", Name = "UQ__LoanAppl__C4B333694F1AB2B2", IsUnique = true)]
public partial class LoanApplication
{
    [Key]
    public int LoanApplicationId { get; set; }

    public int UserId { get; set; }

    [StringLength(10)]
    public string Gender { get; set; } = null!;

    [Column("DOB")]
    public DateOnly Dob { get; set; }

    [StringLength(12)]
    public string Aadhaar { get; set; } = null!;

    [StringLength(500)]
    public string Address { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Income { get; set; }

    [StringLength(50)]
    public string EmploymentType { get; set; } = null!;

    public int LoanProductId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal RequestedAmount { get; set; }

    public int RequestedTenure { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InterestRate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? DecisionDate { get; set; }

    [StringLength(500)]
    public string? DecisionReason { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [InverseProperty("LoanApplication")]
    public virtual ICollection<DecisionAppLog> DecisionAppLogs { get; set; } = new List<DecisionAppLog>();

    [InverseProperty("LoanApplication")]
    public virtual ICollection<GoldLoanApplication> GoldLoanApplications { get; set; } = new List<GoldLoanApplication>();

    [InverseProperty("LoanApplication")]
    public virtual ICollection<HomeLoanApplication> HomeLoanApplications { get; set; } = new List<HomeLoanApplication>();

    [InverseProperty("LoanApplication")]
    public virtual ICollection<LoanBankStatement> LoanBankStatements { get; set; } = new List<LoanBankStatement>();

    [ForeignKey("LoanProductId")]
    [InverseProperty("LoanApplications")]
    public virtual LoanProduct LoanProduct { get; set; } = null!;

    [InverseProperty("LoanApplication")]
    public virtual ICollection<LoanProductDocument> LoanProductDocuments { get; set; } = new List<LoanProductDocument>();

    [InverseProperty("LoanApplication")]
    public virtual ICollection<LoanRepaymentSchedule> LoanRepaymentSchedules { get; set; } = new List<LoanRepaymentSchedule>();

    [InverseProperty("LoanApplication")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    [ForeignKey("UserId")]
    [InverseProperty("LoanApplications")]
    public virtual User User { get; set; } = null!;

    public DateTime StartDate { get; set; }
}
