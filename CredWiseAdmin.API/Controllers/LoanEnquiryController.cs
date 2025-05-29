using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanEnquiryController : ControllerBase
    {
        private readonly ILoanEnquiryService _enquiryService;

        public LoanEnquiryController(ILoanEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpGet("getallenquiry")]
        public async Task<IActionResult> GetAllEnquiries()
        {
            var response = await _enquiryService.GetAllEnquiriesAsync();
            return Ok(response);
        }

        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleEnquiryStatus(int id)
        {
            var response = await _enquiryService.ToggleEnquiryStatusAsync(id);
            return Ok(response);
        }
    }
}
