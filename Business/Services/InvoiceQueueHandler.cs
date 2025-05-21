using Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Business.Azure_Dtos;
using Data.Contexts;

namespace Business.Services
{
    public class InvoiceQueueHandler : IHostedService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider;

        public InvoiceQueueHandler(ServiceBusProcessor processor, IServiceProvider serviceProvider)
        {
            _processor = processor;
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken ct)
        {
            _processor.ProcessMessageAsync += HandleMessageAsync;
            _processor.ProcessErrorAsync += ErrorHandler;
            return _processor.StartProcessingAsync(ct);
        }

        public Task StopAsync(CancellationToken ct)
            => _processor.StopProcessingAsync(ct);

        private async Task HandleMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var msg = JsonSerializer.Deserialize<InvoiceMessageDto>(body)!;

            // replicate your invoice-saving logic
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

            var invoice = new InvoiceEntity
            {
                BookingId = msg.BookingId,
                UserId = msg.UserId,
                UserName = msg.FirstName + " " + msg.LastName,
                UserEmail = msg.BookingEmail,
                UserAddress = msg.BookingAddress,
                UserPhone = msg.BookingPhone,
                EventId = msg.EventId,
                EventName = msg.EventName,
                EventOwnerName = msg.EventOwnerName,
                EventOwnerEmail = msg.EventOwnerEmail,
                EventOwnerAddress = msg.EventOwnerAddress,
                EventOwnerPhone = msg.EventOwnerPhone,
                InvoicePaid = msg.InvoicePaid,
                IssuedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            foreach (var it in msg.Tickets)
            {
                invoice.InvoiceItems.Add(new InvoiceItemEntity
                {
                    TicketCategory = it.TicketCategory,
                    Price = it.Price,
                    Quantity = it.Quantity
                });
            }

            invoice.Subtotal = invoice.InvoiceItems.Sum(i => i.Amount);
            invoice.Tax = invoice.Subtotal * 0.25m;
            invoice.Fee = 10m;
            invoice.Total = invoice.Subtotal + invoice.Tax + invoice.Fee;

            db.Invoices.Add(invoice);
            await db.SaveChangesAsync();

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}
