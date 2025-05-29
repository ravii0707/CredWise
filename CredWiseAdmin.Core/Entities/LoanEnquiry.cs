using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class LoanEnquiry
{
    [Key]
    public int EnquiryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    [StringLength(15)]
    public string Phone { get; set; }

    [Required]
    public string Message { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    [StringLength(500)]
    public string? ResolutionNote { get; set; }
}
