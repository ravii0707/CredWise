using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class ApplyPenaltyDto
    {
        [Required]
        public int RepaymentId { get; set; }
    }
}
