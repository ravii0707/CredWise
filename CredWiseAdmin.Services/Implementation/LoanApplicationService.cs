using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Data.Repositories.Interfaces;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Implementation
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoanBankStatementRepository _bankStatementRepository;
        private readonly ILoanRepaymentRepository _loanRepaymentRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<LoanApplicationService> _logger;

        public LoanApplicationService(
            ILoanApplicationRepository loanApplicationRepository,
            ILoanProductRepository loanProductRepository,
            IUserRepository userRepository,
            ILoanBankStatementRepository bankStatementRepository,
            ILoanRepaymentRepository loanRepaymentRepository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            ILogger<LoanApplicationService> logger)
        {
            _loanApplicationRepository = loanApplicationRepository ?? throw new ArgumentNullException(nameof(loanApplicationRepository));
            _loanProductRepository = loanProductRepository ?? throw new ArgumentNullException(nameof(loanProductRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _bankStatementRepository = bankStatementRepository ?? throw new ArgumentNullException(nameof(bankStatementRepository));
            _loanRepaymentRepository = loanRepaymentRepository ?? throw new ArgumentNullException(nameof(loanRepaymentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoanApplicationResponseDto> CreateLoanApplicationAsync(LoanApplicationDto applicationDto)
        {
            _logger.LogInformation("Creating new loan application for user {UserId}", applicationDto.UserId);

            try
            {
                // Validate input
                if (applicationDto == null)
                {
                    throw new ArgumentNullException(nameof(applicationDto));
                }

                // Check if user exists
                var user = await _userRepository.GetByIdAsync(applicationDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", applicationDto.UserId);
                    throw new KeyNotFoundException("User not found");
                }

                // Check if user already has an active loan
                if (await _loanApplicationRepository.HasActiveLoan(applicationDto.UserId))
                {
                    _logger.LogWarning("User {UserId} already has an active loan", applicationDto.UserId);
                    throw new InvalidOperationException("User already has an active loan");
                }

                // Check if loan product exists
                var loanProduct = await _loanProductRepository.GetByIdAsync(applicationDto.LoanProductId);
                if (loanProduct == null)
                {
                    _logger.LogWarning("Loan product not found with ID: {LoanProductId}", applicationDto.LoanProductId);
                    throw new KeyNotFoundException("Loan product not found");
                }

                // Validate loan amount (minimum ₹10,000)
                if (applicationDto.RequestedAmount < 10000)
                {
                    _logger.LogWarning("Loan amount {Amount} is below minimum threshold", applicationDto.RequestedAmount);
                    throw new ArgumentException("Minimum loan amount is ₹10,000");
                }

                var application = _mapper.Map<LoanApplication>(applicationDto);
                application.Status = "Pending";
                application.DecisionDate = DateTime.UtcNow;
                application.DecisionReason = "Application submitted";
                application.IsActive = true;
                application.CreatedAt = DateTime.UtcNow;
                application.ModifiedAt = DateTime.UtcNow;
                application.CreatedBy = "System";
                application.ModifiedBy = "System";

                await _loanApplicationRepository.AddAsync(application);
                _logger.LogInformation("Successfully created loan application {LoanApplicationId}", application.LoanApplicationId);

                return _mapper.Map<LoanApplicationResponseDto>(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan application for user {UserId}", applicationDto?.UserId);
                throw;
            }
        }

        public async Task<LoanApplicationResponseDto> GetLoanApplicationByIdAsync(int id)
        {
            _logger.LogInformation("Fetching loan application with ID: {LoanApplicationId}", id);

            try
            {
                var application = await _loanApplicationRepository.GetByIdAsync(id);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanApplicationId}", id);
                    throw new KeyNotFoundException("Loan application not found");
                }

                return _mapper.Map<LoanApplicationResponseDto>(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching loan application with ID: {LoanApplicationId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<LoanApplicationResponseDto>> GetLoanApplicationsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching loan applications for user {UserId}", userId);

            try
            {
                var applications = await _loanApplicationRepository.GetByUserIdAsync(userId);
                if (applications == null || !applications.Any())
                {
                    _logger.LogInformation("No loan applications found for user {UserId}", userId);
                    return Enumerable.Empty<LoanApplicationResponseDto>();
                }

                return _mapper.Map<IEnumerable<LoanApplicationResponseDto>>(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching loan applications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<LoanApplicationResponseDto>> GetPendingLoanApplicationsAsync()
        {
            _logger.LogInformation("Fetching all pending loan applications");

            try
            {
                var applications = await _loanApplicationRepository.GetPendingApplicationsAsync();
                return _mapper.Map<IEnumerable<LoanApplicationResponseDto>>(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending loan applications");
                throw;
            }
        }

        public async Task<IEnumerable<LoanApplicationResponseDto>> GetApprovedLoanApplicationsAsync()
        {
            _logger.LogInformation("Fetching all approved loan applications");

            try
            {
                var applications = await _loanApplicationRepository.GetApprovedApplicationsAsync();
                return _mapper.Map<IEnumerable<LoanApplicationResponseDto>>(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching approved loan applications");
                throw;
            }
        }

        public async Task<bool> UploadBankStatementAsync(int loanApplicationId, UploadBankStatementDto uploadDto)
        {
            _logger.LogInformation("Uploading bank statement for loan application {LoanApplicationId}", loanApplicationId);

            try
            {
                if (uploadDto == null)
                {
                    throw new ArgumentNullException(nameof(uploadDto));
                }

                var application = await _loanApplicationRepository.GetByIdAsync(loanApplicationId);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanApplicationId}", loanApplicationId);
                    throw new KeyNotFoundException("Loan application not found");
                }

                // Save file to storage
                var filePath = await _fileStorageService.SaveFileAsync(uploadDto.Document, "bankstatements");

                var bankStatement = new LoanBankStatement
                {
                    LoanApplicationId = loanApplicationId,
                    DocumentName = uploadDto.DocumentName,
                    DocumentPath = filePath,
                    Status = "Pending",
                    RejectionReason = "",
                    VerifiedBy = 0,
                    VerifiedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ModifiedBy = "System"
                };

                await _bankStatementRepository.AddAsync(bankStatement);
                _logger.LogInformation("Successfully uploaded bank statement for loan application {LoanApplicationId}", loanApplicationId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading bank statement for loan application {LoanApplicationId}", loanApplicationId);
                throw;
            }
        }

        public async Task<bool> SendToDecisionAppAsync(int loanApplicationId)
        {
            _logger.LogInformation("Sending loan application {LoanApplicationId} to decision app", loanApplicationId);

            try
            {
                var application = await _loanApplicationRepository.GetByIdAsync(loanApplicationId);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanApplicationId}", loanApplicationId);
                    throw new KeyNotFoundException("Loan application not found");
                }

                application.Status = "Processing";
                application.DecisionReason = "Sent to decision application";
                application.ModifiedAt = DateTime.UtcNow;
                application.ModifiedBy = "System";

                await _loanApplicationRepository.UpdateAsync(application);
                _logger.LogInformation("Successfully sent loan application {LoanApplicationId} to decision app", loanApplicationId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending loan application {LoanApplicationId} to decision app", loanApplicationId);
                throw;
            }
        }

        public async Task<bool> UpdateLoanStatusAsync(int loanApplicationId, string status, string reason)
        {
            _logger.LogInformation("Updating status for loan application {LoanApplicationId} to {Status}", loanApplicationId, status);

            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    throw new ArgumentException("Status cannot be empty", nameof(status));
                }

                var application = await _loanApplicationRepository.GetByIdAsync(loanApplicationId);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanApplicationId}", loanApplicationId);
                    throw new KeyNotFoundException("Loan application not found");
                }

                application.Status = status;
                application.DecisionReason = reason ?? string.Empty;
                application.DecisionDate = DateTime.UtcNow;
                application.ModifiedAt = DateTime.UtcNow;
                application.ModifiedBy = "System";

                await _loanApplicationRepository.UpdateAsync(application);
                _logger.LogInformation("Successfully updated status for loan application {LoanApplicationId}", loanApplicationId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for loan application {LoanApplicationId}", loanApplicationId);
                throw;
            }
        }

        public async Task<bool> FinalizeRepaymentAsync(int loanApplicationId)
        {
            _logger.LogInformation("Finalizing repayment for loan application {LoanApplicationId}", loanApplicationId);

            try
            {
                var application = await _loanApplicationRepository.GetByIdAsync(loanApplicationId);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanApplicationId}", loanApplicationId);
                    throw new KeyNotFoundException("Loan application not found");
                }

                var repayments = await _loanRepaymentRepository.GetByLoanApplicationIdAsync(loanApplicationId);
                if (repayments.Any(r => r.Status != "Paid"))
                {
                    _logger.LogWarning("Cannot finalize - not all repayments completed for loan application {LoanApplicationId}", loanApplicationId);
                    throw new InvalidOperationException("Not all repayments have been completed");
                }

                application.Status = "Completed";
                application.IsActive = false;
                application.ModifiedAt = DateTime.UtcNow;
                application.ModifiedBy = "System";

                await _loanApplicationRepository.UpdateAsync(application);
                _logger.LogInformation("Successfully finalized repayment for loan application {LoanApplicationId}", loanApplicationId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing repayment for loan application {LoanApplicationId}", loanApplicationId);
                throw;
            }
        }

        public async Task<RepaymentPlanResponseDto> GenerateRepaymentPlanAsync(EmiPlanDto emiPlanDto)
        {
            _logger.LogInformation("Generating repayment plan for loan {LoanId}", emiPlanDto.LoanId);

            try
            {
                if (emiPlanDto == null)
                {
                    throw new ArgumentNullException(nameof(emiPlanDto));
                }

                var application = await _loanApplicationRepository.GetByIdAsync(emiPlanDto.LoanId);
                if (application == null)
                {
                    _logger.LogWarning("Loan application not found with ID: {LoanId}", emiPlanDto.LoanId);
                    throw new KeyNotFoundException("Loan application not found");
                }

                if (application.Status != "Approved")
                {
                    _logger.LogWarning("Loan {LoanId} must be approved before generating repayment plan", emiPlanDto.LoanId);
                    throw new InvalidOperationException("Loan must be approved before generating repayment plan");
                }

                // Validate tenure
                if (emiPlanDto.TenureInMonths != 6 && emiPlanDto.TenureInMonths != 12 && emiPlanDto.TenureInMonths != 24)
                {
                    _logger.LogWarning("Invalid tenure {Tenure} months for loan {LoanId}", emiPlanDto.TenureInMonths, emiPlanDto.LoanId);
                    throw new ArgumentException("EMI plan must be 6, 12, or 24 months");
                }

                // Calculate EMI using standard formula: P × r × (1 + r)^n / ((1 + r)^n - 1)
                decimal principal = application.RequestedAmount;
                decimal monthlyInterestRate = (decimal)emiPlanDto.InterestRate / 100 / 12;
                int numberOfPayments = emiPlanDto.TenureInMonths;

                decimal emi = principal * monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, numberOfPayments) /
                            (decimal)(Math.Pow(1 + (double)monthlyInterestRate, numberOfPayments) - 1);

                var repaymentPlan = new RepaymentPlanResponseDto
                {
                    LoanApplicationId = application.LoanApplicationId,
                    PrincipalAmount = principal,
                    InterestRate = (decimal)emiPlanDto.InterestRate,
                    TenureMonths = numberOfPayments,
                    MonthlyEmi = emi,
                    TotalInterest = emi * numberOfPayments - principal,
                    TotalRepayment = emi * numberOfPayments,
                    StartDate = DateTime.UtcNow,
                    Repayments = new List<LoanRepaymentDto>()
                };

                // Generate repayment schedule
                for (int i = 1; i <= numberOfPayments; i++)
                {
                    repaymentPlan.Repayments.Add(new LoanRepaymentDto
                    {
                        InstallmentNumber = i,
                        DueDate = DateTime.UtcNow.AddMonths(i),
                        PrincipalAmount = principal / numberOfPayments,
                        InterestAmount = emi - (principal / numberOfPayments),
                        TotalAmount = emi,
                        Status = "Pending"
                    });
                }

                _logger.LogInformation("Successfully generated repayment plan for loan {LoanId}", emiPlanDto.LoanId);
                return repaymentPlan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating repayment plan for loan {LoanId}", emiPlanDto?.LoanId);
                throw;
            }
        }
    }
}