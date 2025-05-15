using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Azure_Dtos
{
    public class InvoiceMessageDto
    {
        public string BookingId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string UserEmail { get; set; } = default!;
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
        public string EventId { get; set; } = default!;
        public string EventName { get; set; } = default!;
        public string EventOwnerName { get; set; } = default!;
        public string EventOwnerEmail { get; set; } = default!;
        public string EventOwnerAddress { get; set; } = default!;
        public string EventOwnerPhone { get; set; } = default!;
        public bool InvoicePaid { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
