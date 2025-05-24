using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredWiseAdmin.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanRepaymentsController : ControllerBase
    {
        private readonly ILoanRepaymentService _loanRepaymentService;

        public LoanRepaymentsController(ILoanRepaymentService loanRepaymentService)
        {
            _loanRepaymentService = loanRepaymentService;
        }

        [HttpGet("loan/{loanApplicationId}")]
        public async Task<ActionResult<IEnumerable<LoanRepaymentDto>>> GetRepaymentsByLoanId(int loanApplicationId)
        {
            var repayments = await _loanRepaymentService.GetRepaymentsByLoanIdAsync(loanApplicationId);
            return Ok(repayments);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<PaymentTransactionResponseDto>> ProcessPayment([FromBody] PaymentTransactionDto paymentDto)
        {
            var transaction = await _loanRepaymentService.ProcessPaymentAsync(paymentDto);
            return Ok(transaction);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("{repaymentId}/penalty")]
        public async Task<IActionResult> ApplyPenalty(int repaymentId)
        {
            var result = await _loanRepaymentService.ApplyPenaltyAsync(repaymentId);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet("user/{userId}/pending")]
        public async Task<ActionResult<IEnumerable<LoanRepaymentDto>>> GetPendingRepayments(int userId)
        {
            var repayments = await _loanRepaymentService.GetPendingRepaymentsAsync(userId);
            return Ok(repayments);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanRepaymentDto>>> GetOverdueRepayments()
        {
            var repayments = await _loanRepaymentService.GetOverdueRepaymentsAsync();
            return Ok(repayments);
        }
    }
}
