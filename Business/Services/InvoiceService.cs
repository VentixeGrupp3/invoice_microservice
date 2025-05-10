using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Factories;
using Business.Forms;
using Business.Models;
using Data.Entities;
using Data.Repos;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Business.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceModel>> GetInvoicesForUserAsync(string userId);
        Task<IEnumerable<InvoiceModel>> GetAllInvoicesAsync();
        Task<InvoiceResult> GetInvoiceByIdAsync(string invoiceId);
        Task<InvoiceResult> ManuallyCreateInvoiceAsync(ManuallyCreateInvoiceForm form);
    }

    public class InvoiceService(IInvoiceRepo invoiceRepo, IMappingFactory<InvoiceEntity, InvoiceModel> mappingFactory) : IInvoiceService
    {
        private readonly IInvoiceRepo _invoiceRepo = invoiceRepo;
        private readonly IMappingFactory<InvoiceEntity, InvoiceModel> _mappingFactory = mappingFactory;


        public async Task<IEnumerable<InvoiceModel>> GetInvoicesForUserAsync(string userId)
        {
            // Check if any invoices exist for the user
            var hasInvoices = await _invoiceRepo.ExistsAsync(i => i.UserId == userId);
            if (!hasInvoices)
                return Enumerable.Empty<InvoiceModel>();

            var invoices = await _invoiceRepo.GetAllAsync(
                where: i => i.UserId == userId,
                sortBy: i => i.Created,
                orderByDescending: true,
                includes: i => i.InvoiceItems
            );

            return invoices.Select(_mappingFactory.MapToModel);
        }

        public async Task<IEnumerable<InvoiceModel>> GetAllInvoicesAsync()
        {
            var invoices = await _invoiceRepo.GetAllAsync(
                sortBy: i => i.Created,
                orderByDescending: true,
                includes: i => i.InvoiceItems
            );
            return invoices.Select(_mappingFactory.MapToModel);
        }

        public async Task<InvoiceResult> GetInvoiceByIdAsync(string invoiceId)
        {
            var invoice = await _invoiceRepo.GetAsync(
                where: i => i.InvoiceId == invoiceId,
                includes: i => i.InvoiceItems
            );
            if (invoice == null)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Invoice not found."
                };
            }

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 200,
                Invoice = _mappingFactory.MapToModel(invoice)
            };
        }

        public async Task<InvoiceResult> ManuallyCreateInvoiceAsync(ManuallyCreateInvoiceForm form)
        {
            var exists = await _invoiceRepo.ExistsAsync(i => i.BookingId == form.BookingId);
            if (exists)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = $"Invoice already exists for booking {form.BookingId}. Use update instead."
                };
            }
            // Future integration. Use gRPC to check with BookingMicroservice if the booking exists with same details.

            var invoiceItems = form.InvoiceItems.Select(item => new InvoiceItemEntity
            {
                TicketCategory = item.TicketCategory,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();

            var subtotal = invoiceItems.Sum(i => i.Amount);
            var fee = form.CustomFee ?? 5m;
            var taxRate = form.CustomTaxRate ?? 0.25m;
            var tax = subtotal * taxRate;
            var total = subtotal + tax + fee;

            var now = DateTime.UtcNow;

            var invoice = new InvoiceEntity
            {
                BookingId = form.BookingId,
                UserId = form.UserId,
                UserName = form.UserName,
                UserEmail = form.UserEmail,
                UserAddress = form.UserAddress,
                UserPhone = form.UserPhone,
                EventId = form.EventId,
                EventName = form.EventName,
                EventOwnerName = form.EventOwnerName,
                EventOwnerEmail = form.EventOwnerEmail,
                EventOwnerAddress = form.EventOwnerAddress,
                EventOwnerPhone = form.EventOwnerPhone,
                InvoicePaid = form.InvoicePaid,


                IssuedDate = now,
                DueDate = now.AddDays(30),
                Created = now,


                Subtotal = subtotal,
                Tax = tax,
                Fee = fee,
                Total = total,


                InvoiceItems = invoiceItems
            };

            await _invoiceRepo.AddAsync(invoice);

            var invoiceModel = _mappingFactory.MapToModel(invoice);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 201,
                Invoice = invoiceModel
            };
        }
    }
}
