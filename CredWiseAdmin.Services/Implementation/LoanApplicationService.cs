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
        private readonly IEmailService _emailService;

        // Constants for business rules
        private const decimal MIN_LOAN_AMOUNT = 10000;
        private const decimal MAX_LOAN_AMOUNT = 1000000;
        private const decimal MIN_INCOME_MULTIPLIER = 3;
        private const int MIN_AGE = 20;
        private const int MAX_AGE = 100;
        private const decimal MAX_EMI_TO_INCOME_RATIO = 0.6m;

        public LoanApplicationService(
            ILoanApplicationRepository loanApplicationRepository,
            ILoanProductRepository loanProductRepository,
            IUserRepository userRepository,
            ILoanBankStatementRepository bankStatementRepository,
            ILoanRepaymentRepository loanRepaymentRepository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IEmailService emailService,
            ILogger<LoanApplicationService> logger)
        {
            _loanApplicationRepository = loanApplicationRepository ?? throw new ArgumentNullException(nameof(loanApplicationRepository));
            _loanProductRepository = loanProductRepository ?? throw new ArgumentNullException(nameof(loanProductRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _bankStatementRepository = bankStatementRepository ?? throw new ArgumentNullException(nameof(bankStatementRepository));
            _loanRepaymentRepository = loanRepaymentRepository ?? throw new ArgumentNullException(nameof(loanRepaymentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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

                // Log the incoming DTO for debugging
                _logger.LogDebug("Received loan application DTO: {@LoanApplicationDto}", applicationDto);

                // Validate required fields
                if (string.IsNullOrWhiteSpace(applicationDto.Gender))
                    throw new ArgumentException("Gender is required");
                if (string.IsNullOrWhiteSpace(applicationDto.Aadhaar))
                    throw new ArgumentException("Aadhaar number is required");
                if (string.IsNullOrWhiteSpace(applicationDto.Address))
                    throw new ArgumentException("Address is required");
                if (string.IsNullOrWhiteSpace(applicationDto.EmploymentType))
                    throw new ArgumentException("Employment type is required");

                // Validate field lengths
                if (applicationDto.Gender.Length > 10)
                    throw new ArgumentException("Gender must not exceed 10 characters");
                if (applicationDto.Aadhaar.Length != 12)
                    throw new ArgumentException("Aadhaar number must be exactly 12 digits");
                if (applicationDto.Address.Length > 500)
                    throw new ArgumentException("Address must not exceed 500 characters");
                if (applicationDto.EmploymentType.Length > 50)
                    throw new ArgumentException("Employment type must not exceed 50 characters");

                // Validate user
                var user = await _userRepository.GetByIdAsync(applicationDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", applicationDto.UserId);
                    throw new KeyNotFoundException("User not found");
                }

                // Validate age
                var age = DateTime.Today.Year - applicationDto.DOB.Year;
                if (applicationDto.DOB.Date > DateTime.Today.AddYears(-age)) age--;
                if (age < MIN_AGE || age > MAX_AGE)
                {
                    throw new InvalidOperationException($"Age must be between {MIN_AGE} and {MAX_AGE} years");
                }

                // Check if user already has an active loan
                if (await _loanApplicationRepository.HasActiveLoan(applicationDto.UserId))
                {
                    _logger.LogWarning("User {UserId} already has an active loan", applicationDto.UserId);
                    throw new InvalidOperationException("User already has an active loan");
                }

                // Check if Aadhaar is already used
                if (await _loanApplicationRepository.IsAadhaarUsed(applicationDto.Aadhaar))
                {
                    _logger.LogWarning("Aadhaar number {Aadhaar} is already used in another application", applicationDto.Aadhaar);
                    throw new InvalidOperationException("Aadhaar number is already used in another application");
                }

                // Validate loan product
                var loanProduct = await _loanProductRepository.GetByIdAsync(applicationDto.LoanProductId);
                if (loanProduct == null)
                {
                    _logger.LogWarning("Loan product not found with ID: {LoanProductId}", applicationDto.LoanProductId);
                    throw new KeyNotFoundException("Loan product not found");
                }

                // Validate loan amount
                if (applicationDto.RequestedAmount < MIN_LOAN_AMOUNT)
                {
                    throw new ArgumentException($"Minimum loan amount is ₹{MIN_LOAN_AMOUNT:N0}");
                }
                if (applicationDto.RequestedAmount > MAX_LOAN_AMOUNT)
                {
                    throw new ArgumentException($"Maximum loan amount is ₹{MAX_LOAN_AMOUNT:N0}");
                }
                if (applicationDto.RequestedAmount > loanProduct.MaxLoanAmount)
                {
                    throw new ArgumentException($"Loan amount exceeds maximum limit for this product");
                }

                // Validate income
                if (applicationDto.Income < (applicationDto.RequestedAmount / MIN_INCOME_MULTIPLIER))
                {
                    throw new ArgumentException("Income is insufficient for the requested loan amount");
                }

                // Calculate EMI and validate against income
                var emi = CalculateEMI(applicationDto.RequestedAmount, applicationDto.InterestRate, applicationDto.RequestedTenure);
                if (emi > (applicationDto.Income * MAX_EMI_TO_INCOME_RATIO))
                {
                    throw new ArgumentException("EMI exceeds maximum allowed ratio of income");
                }

                try
                {
                    var application = _mapper.Map<LoanApplication>(applicationDto);
                    _logger.LogDebug("Mapped loan application entity: {@LoanApplication}", application);

                    application.Dob = DateOnly.FromDateTime(applicationDto.DOB.Date); // Convert DateTime to DateOnly
                    application.Status = "Pending";
                    application.DecisionDate = DateTime.UtcNow;
                    application.DecisionReason = "Application submitted";
                    application.IsActive = true;
                    application.CreatedAt = DateTime.UtcNow;
                    application.ModifiedAt = DateTime.UtcNow;
                    application.CreatedBy = "Admin";
                    application.ModifiedBy = "Admin";

                    await _loanApplicationRepository.AddAsync(application);
                    _logger.LogInformation("Successfully created loan application {LoanApplicationId}", application.LoanApplicationId);

                    // Send email notification
                    try
                    {
                        await _emailService.SendLoanApprovalEmailAsync(user.Email, application.LoanApplicationId);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send loan application email to {Email}", user.Email);
                    }

                    var responseDto = _mapper.Map<LoanApplicationResponseDto>(application);
                    _logger.LogDebug("Mapped response DTO: {@LoanApplicationResponseDto}", responseDto);
                    return responseDto;
                }
                catch (Exception mappingEx)
                {
                    _logger.LogError(mappingEx, "Error during mapping or saving loan application: {Message}", mappingEx.Message);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan application for user {UserId}: {Message}", applicationDto?.UserId, ex.Message);
                throw;
            }
        }

        private decimal CalculateEMI(decimal principal, decimal interestRate, int tenure)
        {
            var monthlyRate = interestRate / 12 / 100;
            var emi = principal * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, tenure) 
                    / (decimal)(Math.Pow(1 + (double)monthlyRate, tenure) - 1);
            return Math.Round(emi, 2);
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
                    CreatedBy = "Admin",
                    ModifiedBy = "Admin"
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
                application.ModifiedBy = "Admin";

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

                var user = await _userRepository.GetByIdAsync(application.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                application.Status = status;
                application.DecisionReason = reason ?? string.Empty;
                application.DecisionDate = DateTime.UtcNow;
                application.ModifiedAt = DateTime.UtcNow;
                application.ModifiedBy = "Admim";

                await _loanApplicationRepository.UpdateAsync(application);

                // Send appropriate email based on status
                try
                {
                    if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        await _emailService.SendLoanApprovalEmailAsync(user.Email, loanApplicationId);
                    }
                    else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                    {
                        await _emailService.SendLoanRejectionEmailAsync(user.Email, reason ?? "Application rejected");
                    }
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send status update email to {Email}", user.Email);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loan application status {LoanApplicationId}", loanApplicationId);
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
                application.ModifiedBy = "Admin";

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