using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    public class SoftDeleteInvoiceForm
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
        [SwaggerSchema("Clarify reason for invoice needing to be soft deleted, for example, User has deleted account.")]
        public string? DeletionReason { get; set; }
        [Required]
        [SwaggerSchema(" The name of the admin who is deleting the invoice. Admin id can be used also")]
        public string? DeletedBy { get; set; }
    }
}
