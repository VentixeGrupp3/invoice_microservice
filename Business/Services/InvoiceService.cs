using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Factories;
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
    }

    public class InvoiceService(IInvoiceRepo invoiceRepo, IMappingFactory<InvoiceEntity, InvoiceModel> mappingFactory) : IInvoiceService
    {
        private readonly IInvoiceRepo _invoiceRepo = invoiceRepo;
        private readonly IMappingFactory<InvoiceEntity, InvoiceModel> _mappingFactory = mappingFactory;


        public async Task<IEnumerable<InvoiceModel>> GetInvoicesForUserAsync(string userId)
        {
            // Optional: check if any exist
            var hasInvoices = await _invoiceRepo.UserExistsAsync(userId);
            if (!hasInvoices)
                return Enumerable.Empty<InvoiceModel>(); // or throw / return null etc.

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
    }
}
