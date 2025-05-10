using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    public class UpdateInvoiceForm
    {
        [Required]
        [SwaggerSchema("The unique ID of the invoice to update.")]
        public string InvoiceId { get; set; } = null!;

        [Required]
        [SwaggerSchema("The unique ID of the user, used for verification only.")]
        public string UserId { get; set; } = null!; 

        [Required]
        [SwaggerSchema("The unique ID of the Booking, used for verification only.")]
        public string BookingId { get; set; } = null!; 

        [Required]
        [SwaggerSchema("The unique ID of the Event, used for verification only.")]
        public string EventId { get; set; } = null!; 

        [Required]
        public string UserName { get; set; } = null!;
        [Required, EmailAddress]
        public string UserEmail { get; set; } = null!;
        public string? UserAddress { get; set; }
        public string? UserPhone { get; set; }

        [Required]
        public string EventName { get; set; } = null!;
        [Required]
        public string EventOwnerName { get; set; } = null!;
        [Required, EmailAddress]
        public string EventOwnerEmail { get; set; } = null!;
        [Required]
        public string EventOwnerAddress { get; set; } = null!;
        [Required]
        public string EventOwnerPhone { get; set; } = null!;

        public bool InvoicePaid { get; set; }

        [SwaggerSchema("Custom administrative fee (default 5 SEK if empty).")]
        public decimal? CustomFee { get; set; }

        [SwaggerSchema("Custom tax rate (default 25% if empty).")]
        public decimal? CustomTaxRate { get; set; }

        [Required,MinLength(5)]
        public string AdjustedBy { get; set; } = null!;

        [Required, MinLength(5)]
        public string AdjustmentReason { get; set; } = null!;

        [Required, MinLength(1)]
        public List<InvoiceItemForm> InvoiceItems { get; set; } = new();
    }
}
