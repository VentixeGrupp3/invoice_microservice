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
        public string BookingEmail { get; set; } = default!;
        public string? BookingPhone { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        
        public string? BookingAddress { get; set; }
        public string EventId { get; set; } = default!;
        public string EventName { get; set; } = default!;
        public string EventOwnerName { get; set; } = default!;
        public string EventOwnerEmail { get; set; } = default!;
        public string EventOwnerAddress { get; set; } = default!;
        public string EventOwnerPhone { get; set; } = default!;
        public bool InvoicePaid { get; set; }
        public List<InvoiceTicketDto> Tickets { get; set; } = new();
    }
}
