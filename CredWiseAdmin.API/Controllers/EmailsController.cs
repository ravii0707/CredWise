using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CredWiseAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailsController> _logger;

        public EmailsController(IEmailService emailService,ILogger<EmailsController> logger)

        {
            _emailService = emailService;
            _logger = logger;
        }


        /// Send user registration email with credentials
        [Authorize(Roles = "Admin")]
        [HttpPost("send-registration")]
     
        public async Task<IActionResult> SendRegistrationEmail([FromBody] RegistrationEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _emailService.SendUserRegistrationEmailAsync(request.Email, request.Password);
                return Ok(new { Success = true, Message = "Registration email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send registration email");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to send email",
                    Error = ex.Message
                });
            }
        }

  
        /// Send loan approval notification
       
        [HttpPost("send-loan-approval")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendLoanApprovalEmail([FromBody] LoanEmailRequest request)
        {
            await _emailService.SendLoanApprovalEmailAsync(request.Email, request.LoanApplicationId);
            return Ok(new { Message = "Loan approval email sent successfully" });
        }


        /// Send loan rejection notification

        [HttpPost("send-loan-rejection")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendLoanRejectionEmail([FromBody] LoanRejectionRequest request)
        {
            await _emailService.SendLoanRejectionEmailAsync(request.Email, request.Reason);
            return Ok(new { Message = "Loan rejection email sent successfully" });
        }

     
        /// Send payment confirmation
     
        [HttpPost("send-payment-confirmation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendPaymentConfirmationEmail([FromBody] PaymentEmailRequest request)
        {
            await _emailService.SendPaymentConfirmationEmailAsync(request.Email, request.TransactionId);
            return Ok(new { Message = "Payment confirmation email sent successfully" });
        }
    }

    public class RegistrationEmailRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoanEmailRequest
    {
        public string Email { get; set; }
        public int LoanApplicationId { get; set; }
    }

    public class LoanRejectionRequest
    {
        public string Email { get; set; }
        public string Reason { get; set; }
    }

    public class PaymentEmailRequest
    {
        public string Email { get; set; }
        public int TransactionId { get; set; }
    }
}