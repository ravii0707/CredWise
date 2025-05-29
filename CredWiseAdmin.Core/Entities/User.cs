using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

[Index("Email", Name = "UQ__Users__A9D1053424FBC403", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(256)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Fdapplication> Fdapplications { get; set; } = new List<Fdapplication>();

    [InverseProperty("User")]
    public virtual ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();

    [InverseProperty("VerifiedByNavigation")]
    public virtual ICollection<LoanBankStatement> LoanBankStatements { get; set; } = new List<LoanBankStatement>();
}
