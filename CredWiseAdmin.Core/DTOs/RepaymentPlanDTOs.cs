namespace CredWiseAdmin.Core.DTOs
{
    public class RepaymentPlanDTO
    {
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}
