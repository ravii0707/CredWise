using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredWiseAdmin.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FDController : ControllerBase
    {
        private readonly IFDService _fdService;

        public FDController(IFDService fdService)
        {
            _fdService = fdService;
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<FDTypeResponseDto>>> GetAllFDTypes()
        {
            var fdTypes = await _fdService.GetAllFDTypesAsync();
            return Ok(fdTypes);
        }

        [HttpPost("apply")]
        public async Task<ActionResult<FDApplicationResponseDto>> CreateFDApplication([FromBody] FDApplicationDto fdApplicationDto)
        {
            var application = await _fdService.CreateFDApplicationAsync(fdApplicationDto);
            return CreatedAtAction(nameof(GetFDApplicationById), new { id = application.FDApplicationId }, application);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FDApplicationResponseDto>>> GetFDApplicationsByUserId(int userId)
        {
            var applications = await _fdService.GetFDApplicationsByUserIdAsync(userId);
            return Ok(applications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FDApplicationResponseDto>> GetFDApplicationById(int id)
        {
            var application = await _fdService.GetFDApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
