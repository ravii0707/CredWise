using CredWiseAdmin.Core.Entities;

public interface ILoanEnquiryRepository
{
    Task<IEnumerable<LoanEnquiry>> GetAllEnquiriesAsync();
    Task<LoanEnquiry> GetEnquiryByIdAsync(int id);
    Task<bool> ToggleEnquiryStatusAsync(int id);
}
