using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredWiseAdmin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanProductsController : ControllerBase
    {
        private readonly ILoanProductService _loanProductService;

        public LoanProductsController(ILoanProductService loanProductService)
        {
            _loanProductService = loanProductService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanProductResponseDto>>> GetAllLoanProducts()
        {
            var products = await _loanProductService.GetAllLoanProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanProductResponseDto>> GetLoanProductById(int id)
        {
            var product = await _loanProductService.GetLoanProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
