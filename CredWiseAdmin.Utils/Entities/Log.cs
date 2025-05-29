using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class Log
{
    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    [StringLength(20)]
    public string Level { get; set; } = null!;

    [StringLength(50)]
    public string? UserType { get; set; }

    [StringLength(50)]
    public string? UserId { get; set; }

    public string Message { get; set; } = null!;

    [StringLength(255)]
    public string? ApiEndpoint { get; set; }

    [StringLength(20)]
    public string? ApiMethod { get; set; }
}
