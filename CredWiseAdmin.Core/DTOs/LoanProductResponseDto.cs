using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class LoanProductResponseDto
    {
        public int LoanProductId { get; set; }
        public required string ImageUrl { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal MaxLoanAmount { get; set; }
        public required string LoanType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public required PersonalLoanDetailDto PersonalLoanDetail { get; set; }
        public required HomeLoanDetailDto HomeLoanDetail { get; set; }
        public required GoldLoanDetailDto GoldLoanDetail { get; set; }
    }

    public class PersonalLoanDetailDto
    {
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal MinSalaryRequired { get; set; }
    }

    public class HomeLoanDetailDto
    {
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal DownPaymentPercentage { get; set; }
    }

    public class GoldLoanDetailDto
    {
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal ProcessingFee { get; set; }
        public required string GoldPurityRequired { get; set; }
        public required string RepaymentType { get; set; }
    }
}
