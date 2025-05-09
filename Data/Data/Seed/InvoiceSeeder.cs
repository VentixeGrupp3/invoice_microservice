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
                UserId = "user-123",
                UserName = "Vivyan Madigan",
                UserEmail = "vivyan@example.com",
                EventId = "E002",
                EventName = "Hackathon 2024",
                EventOwnerName = "CodeLab",
                EventOwnerEmail = "hello@codelab.se",
                EventOwnerAddress = "Hack Street 1",
                EventOwnerPhone = "0701231111",
                InvoicePaid = false,
                IssuedDate = DateTime.UtcNow.AddDays(-7),
                DueDate = DateTime.UtcNow.AddDays(7),
                Subtotal = 300,
                Tax = 60,
                Fee = 15,
                Total = 375,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Regular", Price = 150, Quantity = 2 }
                }
            };

            var invoice3 = new InvoiceEntity
            {
                BookingId = "B003",
                UserId = "user-123",
                UserName = "Vivyan Madigan",
                UserEmail = "vivyan@example.com",
                EventId = "E003",
                EventName = "DND Night",
                EventOwnerName = "EventMasters",
                EventOwnerEmail = "contact@eventmasters.com",
                EventOwnerAddress = "Fantasy Ave 9",
                EventOwnerPhone = "0701232222",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-3),
                DueDate = DateTime.UtcNow.AddDays(4),
                Subtotal = 300,
                Tax = 60,
                Fee = 15,
                Total = 375,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Entry", Price = 50, Quantity = 2 },
                    new InvoiceItemEntity { TicketCategory = "Snacks", Price = 20, Quantity = 3 },
                    new InvoiceItemEntity { TicketCategory = "Potion Kit", Price = 30, Quantity = 2 }
                }
            };
            var invoice4 = new InvoiceEntity
            {
                BookingId = "B004",
                UserId = "user-456",
                UserName = "Alice Andersson",
                UserEmail = "alice@example.com",
                EventId = "E004",
                EventName = "Yoga Retreat",
                EventOwnerName = "Wellness AB",
                EventOwnerEmail = "info@wellness.se",
                EventOwnerAddress = "Calm Street 22",
                EventOwnerPhone = "0708884444",
                InvoicePaid = false,
                IssuedDate = DateTime.UtcNow.AddDays(-14),
                DueDate = DateTime.UtcNow.AddDays(0),
                Subtotal = 700,
                Tax = 140,
                Fee = 35,
                Total = 875,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Retreat", Price = 300, Quantity = 2 },
                    new InvoiceItemEntity { TicketCategory = "Massage Add-on", Price = 50, Quantity = 2 }
                }
            };
            var invoice5 = new InvoiceEntity
            {
                BookingId = "B005",
                UserId = "user-456",
                UserName = "Alice Andersson",
                UserEmail = "alice@example.com",
                EventId = "E005",
                EventName = "Web Dev Bootcamp",
                EventOwnerName = "CodeCamp",
                EventOwnerEmail = "bootcamp@code.com",
                EventOwnerAddress = "Campus Lane 3",
                EventOwnerPhone = "0701237890",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-20),
                DueDate = DateTime.UtcNow.AddDays(-5),
                Subtotal = 900,
                Tax = 180,
                Fee = 45,
                Total = 1125,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Full Access", Price = 450, Quantity = 2 }
                }
            };
            var invoice6 = new InvoiceEntity
            {
                BookingId = "B006",
                UserId = "user-456",
                UserName = "Alice Andersson",
                UserEmail = "alice@example.com",
                EventId = "E006",
                EventName = "Sewing Workshop",
                EventOwnerName = "CraftyHands",
                EventOwnerEmail = "events@craftyhands.com",
                EventOwnerAddress = "Thread Road 11",
                EventOwnerPhone = "0701122334",
                InvoicePaid = false,
                IssuedDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(5),
                Subtotal = 100,
                Tax = 20,
                Fee = 5,
                Total = 125,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Basic", Price = 100, Quantity = 1 }
                }
            };
            var invoice7 = new InvoiceEntity
            {
                BookingId = "B007",
                UserId = "user-456",
                UserName = "Alice Andersson",
                UserEmail = "alice@example.com",
                EventId = "E007",
                EventName = "Startup Pitch",
                EventOwnerName = "InvestorBridge",
                EventOwnerEmail = "contact@investorbridge.io",
                EventOwnerAddress = "Venture St 42",
                EventOwnerPhone = "0704567890",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(14),
                Subtotal = 400,
                Tax = 80,
                Fee = 10,
                Total = 490,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Presenter", Price = 400, Quantity = 1 }
                }
            };
            var invoice8 = new InvoiceEntity
            {
                BookingId = "B008",
                UserId = "user-789",
                UserName = "Markus Karlsson",
                UserEmail = "markus@example.com",
                UserAddress = "Björkgatan 12",
                UserPhone = "0709998888",
                EventId = "E008",
                EventName = "Photography Class",
                EventOwnerName = "FocusGroup",
                EventOwnerEmail = "info@focusgroup.se",
                EventOwnerAddress = "Lens Blvd 101",
                EventOwnerPhone = "0707771234",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-12),
                DueDate = DateTime.UtcNow.AddDays(0),
                Subtotal = 250,
                Tax = 50,
                Fee = 5,
                Total = 305,
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Standard", Price = 250, Quantity = 1 }
                }
            };

            var invoice9 = new InvoiceEntity
            {
                BookingId = "B009",
                UserId = "user-789",
                UserName = "Markus Karlsson",
                UserEmail = "markus@example.com",
                UserAddress = "Björkgatan 12",
                UserPhone = "0709998888",
                EventId = "E009",
                EventName = "Wine Tasting",
                EventOwnerName = "Vino Vibes",
                EventOwnerEmail = "tasting@vinovibes.com",
                EventOwnerAddress = "Grape Road 9",
                EventOwnerPhone = "0703214567",
                InvoicePaid = false,
                IssuedDate = DateTime.UtcNow.AddDays(-8),
                DueDate = DateTime.UtcNow.AddDays(2),
                Subtotal = 700,
                Tax = 140,
                Fee = 35,
                Total = 875,
                ManuallyAdjusted = true,
                AdjustedBy = "admin@example.com",
                AdjustedDate = DateTime.UtcNow.AddDays(-2),
                AdjustmentReason = "Price correction after promotion",
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Tasting", Price = 300, Quantity = 2 },
                    new InvoiceItemEntity { TicketCategory = "Cheese Add-on", Price = 50, Quantity = 2 }
                }
            };

            var invoice10 = new InvoiceEntity
            {
                BookingId = "B010",
                UserId = "user-789",
                UserName = "Markus Karlsson",
                UserEmail = "markus@example.com",
                UserAddress = "Björkgatan 12",
                UserPhone = "0709998888",
                EventId = "E010",
                EventName = "LAN Party",
                EventOwnerName = "Lan Lords",
                EventOwnerEmail = "host@lanlords.gg",
                EventOwnerAddress = "Pixel Ave 404",
                EventOwnerPhone = "0709990000",
                InvoicePaid = true,
                IssuedDate = DateTime.UtcNow.AddDays(-18),
                DueDate = DateTime.UtcNow.AddDays(-3),
                Subtotal = 150,
                Tax = 30,
                Fee = 10,
                Total = 190,
                IsDeleted = true,
                DeletionReason = "Created in error",
                DeletedAt = DateTime.UtcNow.AddDays(-1),
                DeletedBy = "admin@example.com",
                InvoiceItems = new List<InvoiceItemEntity>
                {
                    new InvoiceItemEntity { TicketCategory = "Pass", Price = 75, Quantity = 2 }
                }
            };


            context.Invoices.AddRange(invoice1, invoice2, invoice3, invoice4, invoice5, invoice6, invoice7, invoice8, invoice9, invoice10);
            context.SaveChanges();
        }

    }
}
