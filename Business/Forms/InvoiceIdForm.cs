using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    [SwaggerSchema("Form used by Users/Admins to request a specific invoice. If a user is requesting an invoice their UserId must match the UserId of the invoice they are searching for.")]
    public class InvoiceIdForm
    {
        [Required]
        [MinLength(1)]
        [SwaggerSchema(
            Description = "The unique identifier of the Invoice ID for the single invoice you want to retrieve. Example: InvoiceId = 02b4beae - a80f - 4a46 - 9273 - 8550d1db91a5",
            Nullable = false
        )]
        public string InvoiceId { get; set; } = null!;

    }
}
