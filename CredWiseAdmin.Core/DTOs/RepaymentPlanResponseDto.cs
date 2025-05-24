using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class RepaymentPlanResponseDto
    {
        public int LoanApplicationId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal MonthlyEmi { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalRepayment { get; set; }
        public DateTime StartDate { get; set; }
        public List<LoanRepaymentDto> Repayments { get; set; }
    }
}
