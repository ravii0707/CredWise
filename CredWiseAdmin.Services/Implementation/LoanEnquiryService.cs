using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CredWiseAdmin.Services.Interfaces;
// using CredWiseAdmin.Models;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using Microsoft.Extensions.Logging;

namespace CredWiseAdmin.Services.Implementation
{
    public class LoanEnquiryService : ILoanEnquiryService
    {
        private readonly ILoanEnquiryRepository _enquiryRepository;
        private readonly ILogger<LoanEnquiryService> _logger;

        public LoanEnquiryService(
            ILoanEnquiryRepository enquiryRepository,
            ILogger<LoanEnquiryService> logger)
        {
            _enquiryRepository = enquiryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<LoanEnquiry>>> GetAllEnquiriesAsync()
        {
            try
            {
                var enquiries = await _enquiryRepository.GetAllEnquiriesAsync();
                
                if (!enquiries.Any())
                {
                    return ApiResponse<IEnumerable<LoanEnquiry>>.CreateSuccess(
                        new List<LoanEnquiry>(),
                        "The list of enquiry is empty"
                    );
                }

                _logger.LogInformation("Service received {Count} enquiries", enquiries.Count());

                return ApiResponse<IEnumerable<LoanEnquiry>>.CreateSuccess(
                    enquiries,
                    "Enquiries retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving enquiries");
                return ApiResponse<IEnumerable<LoanEnquiry>>.CreateError(
                    $"An error occurred while retrieving enquiries: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<bool>> ToggleEnquiryStatusAsync(int id)
        {
            try
            {
                var result = await _enquiryRepository.ToggleEnquiryStatusAsync(id);
                
                if (!result)
                {
                    return ApiResponse<bool>.CreateError("Enquiry not found");
                }

                return ApiResponse<bool>.CreateSuccess(true, "Enquiry status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling enquiry status for ID: {Id}", id);
                return ApiResponse<bool>.CreateError(
                    "An error occurred while updating enquiry status"
                );
            }
        }
    }
}
