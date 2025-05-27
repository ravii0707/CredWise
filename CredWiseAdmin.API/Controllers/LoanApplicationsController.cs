using CredWiseAdmin.Core.DTOs;
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
            var result = await _loanApplicationService.UpdateLoanStatusAsync(id, statusDto.Status, statusDto.Reason);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
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
        [HttpPost("{id}/generate-repayment-plan")]
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
    }
}
