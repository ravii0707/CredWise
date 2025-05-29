using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredWiseAdmin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanApplicationsController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly ILogger<LoanApplicationsController> _logger;

        public LoanApplicationsController(ILoanApplicationService loanApplicationService, ILogger<LoanApplicationsController> logger)
        {
            _loanApplicationService = loanApplicationService;
            _logger = logger;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLoanApplication([FromBody] LoanApplicationDto applicationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var application = await _loanApplicationService.CreateLoanApplicationAsync(applicationDto);
                return CreatedAtAction(nameof(GetLoanApplicationById), new { id = application.LoanApplicationId }, application);
            }
            catch (Exception ex)
            {
                // Log the full error
                _logger.LogError(ex, "Error creating loan application");

                // Return a more detailed error message
                return StatusCode(500, new
                {
                    Message = "An error occurred while creating the loan application",
                    DetailedMessage = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanApplicationResponseDto>> GetLoanApplicationById(int id)
        {
            var application = await _loanApplicationService.GetLoanApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDto>>> GetLoanApplicationsByUserId(int userId)
        {
            var applications = await _loanApplicationService.GetLoanApplicationsByUserIdAsync(userId);
            return Ok(applications);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDto>>> GetPendingLoanApplications()
        {
            var applications = await _loanApplicationService.GetPendingLoanApplicationsAsync();
            return Ok(applications);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDto>>> GetApprovedLoanApplications()
        {
            var applications = await _loanApplicationService.GetApprovedLoanApplicationsAsync();
            return Ok(applications);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/upload-documents")]
        public async Task<IActionResult> UploadBankStatement(int id, [FromForm] UploadBankStatementDto uploadDto)
        {
            var result = await _loanApplicationService.UploadBankStatementAsync(id, uploadDto);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/send-to-decision-app")]
        public async Task<IActionResult> SendToDecisionApp(int id)
        {
            var result = await _loanApplicationService.SendToDecisionAppAsync(id);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/update-status")]
        public async Task<IActionResult> UpdateLoanStatus(int id, [FromBody] UpdateLoanStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = false, message = "Invalid input.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var result = await _loanApplicationService.UpdateLoanStatusAsync(id, statusDto.Status, statusDto.Reason);
                if (!result)
                {
                    return BadRequest(new { status = false, message = "Failed to update loan status. Please check the status value and application existence." });
                }
                return Ok(new { status = true, message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loan application status for id {Id}", id);
                return StatusCode(500, new { status = false, message = "An unexpected error occurred. Please contact support." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/finalize-repayment")]
        public async Task<IActionResult> FinalizeRepayment(int id)
        {
            var result = await _loanApplicationService.FinalizeRepaymentAsync(id);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/generate-EMI-plan")]
        public async Task<ActionResult<RepaymentPlanResponseDto>> GenerateRepaymentPlan(int id, [FromBody] EmiPlanDto emiPlanDto)
        {
            emiPlanDto.LoanId = id;
            var plan = await _loanApplicationService.GenerateRepaymentPlanAsync(emiPlanDto);
            return Ok(plan);
        }
        [Authorize(Roles ="admin")]
        [HttpPost("generate-repayment-plan")]
        public async Task<ActionResult<RepaymentPlanResponseDto>> GenerateRepaymentPlan([FromBody] EmiPlanDto emiPlanDto)
        {
            var plan = await _loanApplicationService.GenerateRepaymentPlanAsync(emiPlanDto);
            return Ok(plan);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDto>>> GetAllLoanApplications()
        {
            try
            {
                _logger.LogInformation("Received request to fetch all loan applications");
                
                var applications = await _loanApplicationService.GetAllLoanApplicationsAsync();
                
                _logger.LogInformation("Successfully retrieved {Count} loan applications", applications.Count());
                return Ok(applications);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "No loan applications found");
                return NotFound(new { message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while fetching loan applications");
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching loan applications");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpGet("{id}/generate-repayment-plan")]
        public async Task<IActionResult> GenerateRepaymentPlan(int id)
        {
            try
            {
                var loanApplication = await _loanApplicationService.GetLoanApplicationByIdAsync(id);
                if (loanApplication == null)
                {
                    return Ok(new RepaymentPlanResponseDto
                    {
                        Success = true,
                        Message = "There are no loan EMI payments",
                        Data = new List<RepaymentPlanDTO>()
                    });
                }

                var emiPlanDto = new EmiPlanDto
                {
                    LoanId = id,
                    LoanAmount = loanApplication.RequestedAmount,
                    InterestRate = loanApplication.InterestRate,
                    TenureInMonths = loanApplication.RequestedTenure,
                    StartDate = DateTime.UtcNow
                };

                var response = await _loanApplicationService.GenerateRepaymentPlanAsync(emiPlanDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating repayment plan for loan application {Id}", id);
                return Ok(new RepaymentPlanResponseDto
                {
                    Success = false,
                    Message = "An error occurred while generating the repayment plan",
                    Data = new List<RepaymentPlanDTO>()
                });
            }
        }
    }
}
