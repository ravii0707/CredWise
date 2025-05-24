using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanProductDocument
{
    [Key]
    public int LoanProductDocumentId { get; set; }

    public int LoanProductId { get; set; }

    [StringLength(100)]
    public string DocumentName { get; set; } = null!;

    public byte[] DocumentContent { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ModifiedAt { get; set; }

    [StringLength(100)]
    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("LoanProductId")]
    [InverseProperty("LoanProductDocuments")]
    public virtual LoanProduct LoanProduct { get; set; } = null!;
}
