using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class InvoiceModel
    {
        public string InvoiceId { get; set; } = null!;
        public string BookingId { get; set; } = null!;
        public string EventId { get; set; } = null!;
        public string UserId { get; set; } = null!;

        // Status
        public bool InvoicePaid { get; set; }

        // Dates
        public DateTime IssuedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime Created { get; set; }

        // Total breakdown
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        public decimal Total { get; set; }

        // Bill From
        public string EventName { get; set; } = null!;
        public string EventOwnerName { get; set; } = null!;
        public string EventOwnerEmail { get; set; } = null!;
        public string EventOwnerAddress { get; set; } = null!;
        public string EventOwnerPhone { get; set; } = null!;

        // Bill To
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? UserAddress { get; set; }
        public string? UserPhone { get; set; }

        // Invoice Items
        public List<InvoiceItemModel> Items { get; set; } = new();

        // Adjustment Metadata
        public bool ManuallyAdjusted { get; set; }
        public string? AdjustedBy { get; set; }
        public DateTime? AdjustedDate { get; set; }
        public string? AdjustmentReason { get; set; }

        // Deletion Metadata
        public bool IsDeleted { get; set; }
        public string? DeletionReason { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
