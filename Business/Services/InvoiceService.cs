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
        Task<InvoiceResult> UpdateInvoiceAsync(UpdateInvoiceForm form);
        Task<InvoiceResult> SoftDeleteInvoiceAsync(SoftDeleteInvoiceForm form);
        Task<InvoiceResult> DeleteInvoiceAsync(string id);
        Task<InvoiceResult> MarkInvoicePaidAsync(string invoiceId, string userId);
        Task<InvoiceResult> MarkInvoicePaidAsAdminAsync(string invoiceId);
    }

    public class InvoiceService(
        IInvoiceRepo invoiceRepo,
        IMappingFactory<InvoiceEntity, InvoiceModel> mappingFactory,
        IUpdateMappingFactory<InvoiceEntity, UpdateInvoiceForm> updateMappingFactory,
        IUpdateMappingFactory<InvoiceEntity, SoftDeleteInvoiceForm> softDeleteMappingFactory) : IInvoiceService
    {
        private readonly IInvoiceRepo _invoiceRepo = invoiceRepo;
        private readonly IMappingFactory<InvoiceEntity, InvoiceModel> _mappingFactory = mappingFactory;
        private readonly IUpdateMappingFactory<InvoiceEntity, UpdateInvoiceForm> _updateMappingFactory = updateMappingFactory;
        private readonly IUpdateMappingFactory<InvoiceEntity, SoftDeleteInvoiceForm> _softDeleteMapper = softDeleteMappingFactory;


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

        public async Task<InvoiceResult> UpdateInvoiceAsync(UpdateInvoiceForm form)
        {
            var invoice = await _invoiceRepo.GetAsync(
                i => i.InvoiceId == form.InvoiceId,
                i => i.InvoiceItems);

            if (invoice == null)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Invoice not found."
                };
            }
            // Maybe? Future integration. Use gRPC to check with BookingMicroservice if the booking exists with same details.

            // Validation check to ensure IDs match
            if (invoice.UserId != form.UserId ||
                invoice.BookingId != form.BookingId ||
                invoice.EventId != form.EventId)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Provided IDs do not match the invoice record. Update aborted to prevent data inconsistency."
                };
            }

            _updateMappingFactory.MapFormToExistingEntity(form, invoice);

            await _invoiceRepo.UpdateAsync(invoice);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 200,
                Invoice = _mappingFactory.MapToModel(invoice)
            };
        }
        public async Task<InvoiceResult> SoftDeleteInvoiceAsync(SoftDeleteInvoiceForm form)
        {
            var invoice = await _invoiceRepo.GetAsync(
                i => i.InvoiceId == form.InvoiceId,
                i => i.InvoiceItems);

            if (invoice == null)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Invoice not found."
                };

            // verify user/booking/event IDs match to prevent accidents
            if (invoice.UserId != form.UserId ||
                invoice.BookingId != form.BookingId ||
                invoice.EventId != form.EventId)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Provided IDs do not match the invoice record. Delete aborted."
                };
            }

            _softDeleteMapper.MapFormToExistingEntity(form, invoice);

            await _invoiceRepo.UpdateAsync(invoice);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 200,
                Invoice = _mappingFactory.MapToModel(invoice)
            };
        }
        public async Task<InvoiceResult> DeleteInvoiceAsync(string id)
        {
            var invoice = await _invoiceRepo.GetIncludingDeletedAsync(i => i.InvoiceId == id);
            if (invoice == null)
            {
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = $"Invoice with ID '{id}' was not found."
                };
            }

            await _invoiceRepo.HardDeleteAsync(id);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 204
            };
        }

        public async Task<InvoiceResult> MarkInvoicePaidAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetAsync(
                i => i.InvoiceId == invoiceId,
                i => i.InvoiceItems);

            if (invoice == null)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Invoice not found."
                };

            if (invoice.UserId != userId)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 403,
                    ErrorMessage = "You do not have permission to pay this invoice."
                };

            if (invoice.InvoicePaid)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Invoice is already marked as paid."
                };

            // FUTURE IDEA call BookingMicroservice (via gRPC or HTTP) to mark the underlying booking as paid
            // e.g. await _bookingClient.MarkBookingPaidAsync(invoice.BookingId);

            invoice.InvoicePaid = true;
            await _invoiceRepo.UpdateAsync(invoice);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 200,
                Invoice = _mappingFactory.MapToModel(invoice)
            };
        }

        public async Task<InvoiceResult> MarkInvoicePaidAsAdminAsync(string invoiceId)
        {
            var invoice = await _invoiceRepo.GetAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Invoice not found."
                };

            if (invoice.InvoicePaid)
                return new InvoiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Invoice is already marked as paid."
                };

            // FUTURE IDEA call BookingMicroservice (via gRPC or HTTP) to mark the underlying booking as paid
            // // e.g. await _bookingClient.MarkBookingPaidAsync(invoice.BookingId);

            invoice.InvoicePaid = true;
            await _invoiceRepo.UpdateAsync(invoice);

            return new InvoiceResult
            {
                Success = true,
                StatusCode = 200,
                Invoice = _mappingFactory.MapToModel(invoice)
            };
        }
    }
}
