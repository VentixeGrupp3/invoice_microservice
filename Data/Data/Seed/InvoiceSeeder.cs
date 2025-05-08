using Data.Contexts;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Data.Seed
{
    public static class InvoiceSeeder
    {
        public static void SeedTestInvoices(InvoiceDbContext context)
        {
            if (context.Invoices.Any()) return; // Prevent seeding multiple times

            var invoice1 = new InvoiceEntity
            {
                BookingId = "B001",
                UserId = "user-123",
                UserName = "Vivyan Madigan",
                UserEmail = "vivyan@example.com",
                UserAddress = "Testvägen 1",
                UserPhone = "0701234567",
                EventId = "E001",
                EventName = "Test Event 1",
                EventOwnerName = "EventCorp",
                EventOwnerEmail = "info@eventcorp.com",
                EventOwnerAddress = "Gatan 123",
                EventOwnerPhone = "0700000000",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(10),
                Subtotal = 500,
                Tax = 100,
                Fee = 50,
                Total = 650,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity
                    {
                        TicketCategory = "VIP",
                        Price = 250,
                        Quantity = 2
                    }
                }
            };

            var invoice2 = new InvoiceEntity
            {
                BookingId = "B002",
                UserId = "user-456",
                UserName = "Alice Andersson",
                UserEmail = "alice@example.com",
                EventId = "E002",
                EventName = "Test Event 2",
                EventOwnerName = "MegaEvents",
                EventOwnerEmail = "hello@megaevents.se",
                EventOwnerAddress = "Box 100",
                EventOwnerPhone = "0701237890",
                InvoicePaid = false,
                IssuedDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(5),
                Subtotal = 300,
                Tax = 60,
                Fee = 20,
                Total = 380,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity
                    {
                        TicketCategory = "Regular",
                        Price = 150,
                        Quantity = 2
                    }
                }
            };

            context.Invoices.AddRange(invoice1, invoice2);
            context.SaveChanges();
        }

    }
}
