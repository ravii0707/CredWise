using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanEnquiriesController : ControllerBase
    {
        private readonly ILoanEnquiryService _loanEnquiryService;
        private readonly ILogger<LoanEnquiriesController> _logger;

        public LoanEnquiriesController(
            ILoanEnquiryService loanEnquiryService,
            ILogger<LoanEnquiriesController> logger)
        {
            _loanEnquiryService = loanEnquiryService ?? throw new ArgumentNullException(nameof(loanEnquiryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEnquiries()
        {
            try
            {
                var enquiries = await _loanEnquiryService.GetAllEnquiriesAsync();
                
                if (!enquiries.Any())
                {
                    return Ok(new
                    {
                        status = true,
                        message = "There are no loan enquiries",
                        data = new List<LoanEnquiryResponseDto>()
                    });
                }

                return Ok(new
                {
                    status = true,
                    message = "Successfully retrieved loan enquiries",
                    data = enquiries
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all loan enquiries");
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while retrieving loan enquiries",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEnquiryById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Invalid enquiry ID. Please provide a valid positive number."
                    });
                }

                var enquiry = await _loanEnquiryService.GetEnquiryByIdAsync(id);
                if (enquiry == null)
                {
                    return Ok(new
                    {
                        status = true,
                        message = $"No loan enquiry found with ID {id}",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    status = true,
                    message = "Successfully retrieved loan enquiry",
                    data = enquiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving loan enquiry with ID {EnquiryId}", id);
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while retrieving the loan enquiry",
                    error = ex.Message
                });
            }
        }
    }
} 