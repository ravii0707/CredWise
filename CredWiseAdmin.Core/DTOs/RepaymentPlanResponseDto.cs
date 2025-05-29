using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class RepaymentPlanResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<RepaymentPlanDTO> Data { get; set; }
    }
}
