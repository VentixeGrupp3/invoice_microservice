using Business.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    [SwaggerSchema("Form used to manually create an invoice with essential data. This form is only used if Azure Service Bus has failed or a booking is created manually by an Admin. Fields such as invoiceId, created date, totals will be automatically calculated.")]
    public class ManuallyCreateInvoiceForm
    {
        [Required, MinLength(1)]
        [SwaggerSchema("Booking ID related to this invoice")]
        public string BookingId { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("User ID for whom the invoice is issued")]
        public string UserId { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("Event ID related to this invoice")]
        public string EventId { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("Name of the user (invoice recipient)")]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        [SwaggerSchema("Email of the user (invoice recipient)")]
        public string UserEmail { get; set; } = null!;

        [SwaggerSchema("Address of the user (optional)")]
        public string? UserAddress { get; set; }

        [SwaggerSchema("Phone number of the user (optional)")]
        public string? UserPhone { get; set; }

        [Required, MinLength(1)]
        [SwaggerSchema("Name of the event")]
        public string EventName { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("Event owner's company name")]
        public string EventOwnerName { get; set; } = null!;

        [Required, EmailAddress]
        [SwaggerSchema("Email address of the event owner")]
        public string EventOwnerEmail { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("Address of the event owner")]
        public string EventOwnerAddress { get; set; } = null!;

        [Required, MinLength(1)]
        [SwaggerSchema("Phone number of the event owner")]
        public string EventOwnerPhone { get; set; } = null!;

        [SwaggerSchema("Has the invoice already been paid at creation? (default: false)")]
        public bool InvoicePaid { get; set; } = false;

        [Required]
        [SwaggerSchema("Invoice items (tickets, products, etc")]
        public List<InvoiceItemForm> InvoiceItems { get; set; } = new();

        [SwaggerSchema("Custom fee in SEK (default 50 SEK)")]
        public decimal? CustomFee { get; set; }

        [SwaggerSchema("Custom tax rate (default 25%)")]
        public decimal? CustomTaxRate { get; set; }
    }
}

