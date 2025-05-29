using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;

public interface ILoanEnquiryService
{
    Task<ApiResponse<IEnumerable<LoanEnquiry>>> GetAllEnquiriesAsync();
    Task<ApiResponse<bool>> ToggleEnquiryStatusAsync(int id);
}
