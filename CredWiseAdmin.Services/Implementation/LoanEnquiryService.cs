using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Implementation
{
    public class LoanEnquiryService : ILoanEnquiryService
    {
        private readonly ILoanEnquiryRepository _loanEnquiryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanEnquiryService> _logger;

        public LoanEnquiryService(
            ILoanEnquiryRepository loanEnquiryRepository,
            IMapper mapper,
            ILogger<LoanEnquiryService> logger)
        {
            _loanEnquiryRepository = loanEnquiryRepository ?? throw new ArgumentNullException(nameof(loanEnquiryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<LoanEnquiryResponseDto>> GetAllEnquiriesAsync()
        {
            _logger.LogInformation("Fetching all loan enquiries");

            try
            {
                var enquiries = await _loanEnquiryRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<LoanEnquiryResponseDto>>(enquiries ?? Enumerable.Empty<LoanEnquiry>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all loan enquiries");
                throw new ServiceException(
                    "Unable to retrieve the list of loan enquiries. Please try again later or contact support if the problem persists.",
                    ex);
            }
        }

        public async Task<LoanEnquiryResponseDto> GetEnquiryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching loan enquiry with ID {EnquiryId}", id);

            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException("Invalid enquiry ID. Please provide a valid positive number.");
                }

                var enquiry = await _loanEnquiryRepository.GetByIdAsync(id);
                if (enquiry == null)
                {
                    return null; // Return null instead of throwing exception to match controller's behavior
                }

                return _mapper.Map<LoanEnquiryResponseDto>(enquiry);
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching loan enquiry with ID {EnquiryId}", id);
                throw new ServiceException(
                    $"Unable to retrieve the loan enquiry details. Please try again later or contact support if the problem persists.",
                    ex);
            }
        }
    }
} 