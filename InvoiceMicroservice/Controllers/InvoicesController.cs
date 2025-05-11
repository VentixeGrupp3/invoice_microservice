using Business.Forms;
using Business.Services;
using InvoiceMicroservice.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Business.Models;

namespace InvoiceMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

        #region User Endpoints

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

        [ApiKeyAuthorize("User")]
        [HttpGet("user-invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceById(string invoiceId)
        {

            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID header is missing.");


            var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            if (result.Invoice.UserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view this invoice. Your UserId does NOT match the UserId on the Invoice you requested.");

            return Ok(result.Invoice);
        }

        #endregion

        #region Admin Endpoints

        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-get-users-invoices")]
        public async Task<IActionResult> GetUsersInvoices(string userId)
        {

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


        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-get-user-invoice/{invoiceId}")]
        public async Task<IActionResult> GetUserInvoiceById(string invoiceId)
        {
            var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Invoice);
        }


        [ApiKeyAuthorize("Admin")]
        [HttpPost("admin-invoice-creation")]
        public async Task<IActionResult> ManuallyCreateInvoice(ManuallyCreateInvoiceForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _invoiceService.ManuallyCreateInvoiceAsync(form);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return CreatedAtAction(nameof(GetUserInvoiceById), new { invoiceId = result.Invoice.InvoiceId }, result.Invoice);
        }

        [ApiKeyAuthorize("Admin")]
        [HttpPut("admin-update-invoice")]
        public async Task<IActionResult> UpdateInvoice(UpdateInvoiceForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _invoiceService.UpdateInvoiceAsync(form);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Invoice);
        }

        [ApiKeyAuthorize("Admin")]
        [HttpPut("admin-soft-delete-invoice")]
        public async Task<IActionResult> SoftDeleteInvoice(SoftDeleteInvoiceForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _invoiceService.SoftDeleteInvoiceAsync(form);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Invoice);
        }
        [HttpDelete("admin-hard-delete-invoice/{id}")]
        public async Task<IActionResult> HardDeleteInvoice(string id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);
            return NoContent();
        }

        #endregion
    }
}
