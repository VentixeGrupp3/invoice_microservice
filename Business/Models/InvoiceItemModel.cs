using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class InvoiceItemModel
    {
        public int Id { get; set; } 
        public string TicketCategory { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal Amount => Price * Quantity;
    }
}
