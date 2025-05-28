using System;
using System.Threading.Tasks;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace CredWiseAdmin.Services
{
    public class LoanEnquiryService
    {
        private readonly ILogger<LoanEnquiryService> _logger;
        private readonly ILoanApplicationRepository _repository;

        public LoanEnquiryService(ILogger<LoanEnquiryService> logger, ILoanApplicationRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<bool> SendToDecisionAppAsync(int id)
        {
            try
            {
                var application = await _repository.GetByIdAsync(id);
                if (application == null)
                    throw new NotFoundException($"Loan application with ID {id} not found.");

                if (application.Status != "Initial Review")
                    throw new BadRequestException("Loan application is not in a valid state to send to decision app.");

                // Simulate sending to decision app
                // ...

                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending loan application to decision app");
                throw new ServiceException("An unexpected error occurred while sending to decision app.", ex);
            }
        }
    }
} 