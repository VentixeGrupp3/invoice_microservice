using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Azure_Dtos
{
    public class InvoiceItemDto
    {
        public string TicketCategory { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
