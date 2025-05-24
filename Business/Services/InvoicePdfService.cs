using System;
using Business.Models;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Business.Services
{
    public interface IInvoicePdfService
    {
        byte[] Generate(InvoiceModel invoice);
    }

    public class InvoicePdfService : IInvoicePdfService
    {
        public byte[] Generate(InvoiceModel invoice)
        {
            // Ensure license is set at application startup:
            // QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // --- Page setup ---
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // --- HEADER: EventName + Invoice metadata ---
                    page.Header()
                        .Row(row =>
                        {
                            // Left: Event name & owner info
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(invoice.EventName)
                                    .Bold()
                                    .FontSize(20)
                                    .FontColor(Colors.Blue.Medium);

                                col.Item().PaddingBottom(5);

                            });

                            // Right: Invoice ID, dates
                            row.ConstantItem(150).Column(col =>
                            {
                                col.Item().AlignRight().Text("INVOICE").SemiBold();
                                col.Item().AlignRight().Text($"#{invoice.InvoiceId}");
                                col.Item().AlignRight().Text($"Issued: {invoice.IssuedDate:yyyy-MM-dd}");
                                if (invoice.DueDate is DateTime dd)
                                    col.Item().AlignRight().Text($"Due: {dd:yyyy-MM-dd}");
                            });
                        });



                    // --- CONTENT: Bill From / Bill To + Items table + Notes ---
                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        // Bill From / Bill To
                        col.Item().Row(r =>
                        {
                            r.ConstantItem(260).Column(from =>
                            {
                                from.Item().Text("Bill From").SemiBold();
                                from.Item().Text(invoice.EventOwnerName);
                                from.Item().Text(invoice.EventOwnerAddress);
                                from.Item().Text(invoice.EventOwnerEmail);
                                from.Item().Text(invoice.EventOwnerPhone);
                            });

                            r.RelativeItem().Column(to =>
                            {
                                to.Item().Text("Bill To").SemiBold();
                                to.Item().Text(invoice.UserName);
                                if (!string.IsNullOrEmpty(invoice.UserAddress))
                                    to.Item().Text(invoice.UserAddress);
                                if (!string.IsNullOrEmpty(invoice.UserEmail))
                                    to.Item().Text(invoice.UserEmail);
                                if (!string.IsNullOrEmpty(invoice.UserPhone))
                                    to.Item().Text(invoice.UserPhone);
                            });
                        });

                        // Items table
                        col.Item().Table(table =>
                        {
                            // columns: description, unit price, qty, amount
                            table.ColumnsDefinition(cd =>
                            {
                                cd.RelativeColumn();
                                cd.ConstantColumn(80);
                                cd.ConstantColumn(40);
                                cd.ConstantColumn(80);
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Ticket Type");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Unit Price");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                header.Cell().Element(CellStyle).AlignRight().Text("Amount");

                                static IContainer CellStyle(IContainer c) =>
                                    c.DefaultTextStyle(x => x.SemiBold()).Padding(5).BorderBottom(1);
                            });

                            // Data rows
                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Element(c => c.Padding(5)).Text(item.TicketCategory);
                                table.Cell().Element(c => c.Padding(5)).AlignCenter().Text(item.Price.ToString("C"));
                                table.Cell().Element(c => c.Padding(5)).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Element(c => c.Padding(5)).AlignRight().Text(item.Amount.ToString("C"));
                            }

                            // Footer totals
                            table.Footer(footer =>
                            {
                                footer.Cell().Column(colTotals =>
                                {
                                    colTotals.Item().AlignRight().Text("Subtotal:");
                                    colTotals.Item().AlignRight().Text("Tax:");
                                    colTotals.Item().AlignRight().Text("Fee:");
                                    colTotals.Item().AlignRight().Text("Total:").SemiBold();
                                });

                                footer.Cell().Column(_ => { });
                                footer.Cell().Column(_ => { });

                                footer.Cell().Column(colValues =>
                                {
                                    colValues.Item().AlignRight().Text(invoice.Subtotal.ToString("C"));
                                    colValues.Item().AlignRight().Text(invoice.Tax.ToString("C"));
                                    colValues.Item().AlignRight().Text(invoice.Fee.ToString("C"));
                                    colValues.Item().AlignRight().Text(invoice.Total.ToString("C")).SemiBold();
                                });
                            });
                        });

                        // Optional notes
                        if (invoice.AdjustedDate is DateTime adj)
                        {
                            col.Item().Text($"Adjusted on {adj:yyyy-MM-dd} by {invoice.AdjustedBy}")
                                .Italic()
                                .FontSize(9);
                        }

                        if (invoice.IsDeleted && invoice.DeletedAt is DateTime del)
                        {
                            col.Item().Text($"Deleted on {del:yyyy-MM-dd}, reason: {invoice.DeletionReason}")
                                .Italic()
                                .FontSize(9)
                                .FontColor(Colors.Red.Medium);
                        }
                    });

                    // --- FOOTER: Thank you note ---
                    page.Footer()
                        .AlignCenter()
                        .BorderTop(1)
                        .PaddingTop(5)
                        .Text("Thank you for your business!")
                        .FontSize(9);
                });
            })
            .GeneratePdf();
        }
    }
}
