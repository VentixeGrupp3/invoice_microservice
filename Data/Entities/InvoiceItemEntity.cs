using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class InvoiceItemEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string InvoiceId { get; set; } = null!;
        [ForeignKey(nameof(InvoiceId))]
        public virtual InvoiceEntity Invoice { get; set; } = null!;
        [Required]

        public string TicketCategory { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        // Amount is derived from Price and Quantity.
        // It's marked [NotMapped] to prevent EF Core from trying to map it to a database column.
        [NotMapped]
        public decimal Amount => Price * Quantity;
    }
}
