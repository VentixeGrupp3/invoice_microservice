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

        [HttpGet("user-invoices")]
        public async Task <IActionResult> GetMyInvoices()
        {
            var role = HttpContext.Items["Role"]?.ToString();
            if (role != "User")
                return Forbid("Access denied. Only users may access this endpoint.");

            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID header is missing.");

            var invoices = await _invoiceService.GetInvoicesForUserAsync(userId);

            if (!invoices.Any())
                return NotFound($"No invoices found for user {userId}");

            return Ok(invoices);
        }
    }
}
