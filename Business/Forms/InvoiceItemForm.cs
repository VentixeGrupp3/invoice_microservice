using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    public class InvoiceItemForm
    {
        [Required, MinLength(1)]
        [SwaggerSchema("Ticket or product category")]
        public string TicketCategory { get; set; } = null!;

        [Required, Range(0, 100000)]
        [SwaggerSchema("Price per unit")]
        public decimal Price { get; set; }

        [Required, Range(1, 1000)]
        [SwaggerSchema("Number of units")]
        public int Quantity { get; set; }
    }
}
