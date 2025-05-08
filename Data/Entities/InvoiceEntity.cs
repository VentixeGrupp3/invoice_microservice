using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class InvoiceEntity
    {
        [Key]
        public string InvoiceId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string BookingId { get; set; } = null!;
        [Required]
        public string UserId { get; set; } = null!;

        // User (bill-to) snapshot
        [Required]
        public string UserName { get; set; } = null!; // snapshot
        [Required]
        public string UserEmail { get; set; } = null!; // snapshot
        public string? UserAddress { get; set; }  // snapshot
        public string? UserPhone { get; set; }  // snapshot

        [Required]
        public string EventId { get; set; } = null!;
        [Required]
        public string EventName { get; set; } = null!; // snapshot

        // Event owner (company) snapshot
        [Required]
        public string EventOwnerName { get; set; } = null!; // snapshot
        [Required]
        public string EventOwnerEmail { get; set; } = null!; // snapshot
        [Required]
        public string EventOwnerAddress { get; set; } = null!;  // snapshot
        [Required]
        public string EventOwnerPhone { get; set; } = null!;  // snapshot

        // Simple Boolean for if the event was paid for at the time of booking
        public bool InvoicePaid { get; set; } // snapshot

        public DateTime IssuedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        // Total includes Subtotal + Tax + Fee

        public decimal Total { get; set; }

        // Fields for tracking updates and "deletion" of an invoice entity

        public bool ManuallyAdjusted { get; set; } = false;
        public string? AdjustedBy { get; set; }
        public DateTime? AdjustedDate { get; set; }
        public string? AdjustmentReason { get; set; }

        public bool IsDeleted { get; set; } = false;
        public string? DeletionReason { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        
        [Required]
        public ICollection<InvoiceItemEntity> InvoiceItems { get; set; }
            = new List<InvoiceItemEntity>();

    }
}
