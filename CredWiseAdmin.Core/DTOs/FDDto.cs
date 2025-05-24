using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class FDTypeDto
    {
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal InterestRate { get; set; }

        [Required]
        public decimal MinAmount { get; set; }

        [Required]
        public decimal MaxAmount { get; set; }

        [Required]
        public int Duration { get; set; } // in months
    }

    public class FDTypeResponseDto : FDTypeDto
    {
        public int FDTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FDApplicationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int FDTypeId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int Duration { get; set; } // in months
    }

    public class FDApplicationResponseDto : FDApplicationDto
    {
        public int FDApplicationId { get; set; }
        public decimal InterestRate { get; set; }
        public string? Status { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FDTypeName { get; set; }
    }
}
