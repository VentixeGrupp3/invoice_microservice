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
    public class InvoicesController(IInvoiceService invoiceService, IInvoicePdfService pdfService) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly IInvoicePdfService _pdfService = pdfService;

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
                return NotFound(new List<InvoiceModel>());

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

        [ApiKeyAuthorize("User")]
        [HttpPut("user-pay-invoice/{invoiceId}")]
        public async Task<IActionResult> PayInvoice(string invoiceId)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID header is missing.");

            var result = await _invoiceService.MarkInvoicePaidAsync(invoiceId, userId);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Invoice);
        }

        /// Endpoint to get the PDF of an invoice
        [ApiKeyAuthorize("User")]
        [HttpGet("user-download-invoice/{invoiceId}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(string invoiceId)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID header is missing.");
            // 1) Call service
            var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

            // 2) Handle not-found / errors
            if (!result.Success || result.Invoice is null)
                return StatusCode(result.StatusCode, result.ErrorMessage ?? "Unable to generate PDF");

            // 3) Generate PDF from the InvoiceModel
            byte[] pdfBytes = _pdfService.Generate(result.Invoice);

            // 4) Stream it back as application/pdf attachment
            return File(
                pdfBytes,
                contentType: "application/pdf",
                fileDownloadName: $"invoice_{invoiceId}.pdf"
            );
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
        [HttpPut("admin-pay-invoice/{invoiceId}")]
        public async Task<IActionResult> AdminPayInvoice(string invoiceId)
        {
            var result = await _invoiceService.MarkInvoicePaidAsAdminAsync(invoiceId);

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
        [ApiKeyAuthorize("Admin")]
        [HttpDelete("admin-hard-delete-invoice/{id}")]
        public async Task<IActionResult> HardDeleteInvoice(string id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);
            return NoContent();
        }

        /// Endpoint to get the PDF of an invoice
        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-download-invoice/{invoiceId}/pdf")]
        public async Task<IActionResult> AdminGetInvoicePdf(string invoiceId)
        {
            // 1) Call service
            var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

            // 2) Handle not-found / errors
            if (!result.Success || result.Invoice is null)
                return StatusCode(result.StatusCode, result.ErrorMessage ?? "Unable to generate PDF");

            // 3) Generate PDF from the InvoiceModel
            byte[] pdfBytes = _pdfService.Generate(result.Invoice);

            // 4) Stream it back as application/pdf attachment
            return File(
                pdfBytes,
                contentType: "application/pdf",
                fileDownloadName: $"invoice_{invoiceId}.pdf"
            );
        }


        #endregion
    }
}
