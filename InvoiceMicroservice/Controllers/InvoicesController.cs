using Business.Forms;
using Business.Services;
using InvoiceMicroservice.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

        [ApiKeyAuthorize("User")]
        [HttpGet("user-invoices")]
        public async Task<IActionResult> GetMyInvoices()
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID header is missing.");

            var invoices = await _invoiceService.GetInvoicesForUserAsync(userId);

            if (!invoices.Any())
                return NotFound($"No invoices found for user {userId}");

            return Ok(invoices);
        }

        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-get-users-invoices")]
        public async Task<IActionResult> GetUsersInvoices(UserIdForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = form.UserId;

            var invoices = await _invoiceService.GetInvoicesForUserAsync(userId);

            if (!invoices.Any())
                return NotFound($"No invoices found for user {userId}");

            return Ok(invoices);
        }

        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-get-all-invoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            if (!invoices.Any())
                return NotFound("No invoices found.");

            return Ok(invoices);
        }
    }
}
