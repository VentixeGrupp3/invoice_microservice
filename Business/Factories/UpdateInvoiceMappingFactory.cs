using Business.Models;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Forms;

namespace Business.Factories
{
    public class UpdateInvoiceMappingFactory : IUpdateMappingFactory<InvoiceEntity, UpdateInvoiceForm>
    {
        public void MapFormToExistingEntity(UpdateInvoiceForm form, InvoiceEntity existingEntity)
        {
            existingEntity.UserName = form.UserName;
            existingEntity.UserEmail = form.UserEmail;
            existingEntity.UserAddress = form.UserAddress;
            existingEntity.UserPhone = form.UserPhone;

            existingEntity.EventName = form.EventName;
            existingEntity.EventOwnerName = form.EventOwnerName;
            existingEntity.EventOwnerEmail = form.EventOwnerEmail;
            existingEntity.EventOwnerAddress = form.EventOwnerAddress;
            existingEntity.EventOwnerPhone = form.EventOwnerPhone;

            existingEntity.InvoicePaid = form.InvoicePaid;

            // Date updates
            var now = DateTime.UtcNow;
            existingEntity.IssuedDate = now;
            existingEntity.DueDate = now.AddDays(30);

            // Adjustments and audit info
            existingEntity.ManuallyAdjusted = true;
            existingEntity.AdjustedBy = form.AdjustedBy;
            existingEntity.AdjustedDate = now;
            existingEntity.AdjustmentReason = form.AdjustmentReason;

            // Handle invoice items explicitly here
            existingEntity.InvoiceItems.Clear();
            foreach (var item in form.InvoiceItems)
            {
                existingEntity.InvoiceItems.Add(new InvoiceItemEntity
                {
                    TicketCategory = item.TicketCategory,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            // Recalculate totals
            var subtotal = existingEntity.InvoiceItems.Sum(i => i.Amount);
            var fee = form.CustomFee ?? 5m;
            var taxRate = form.CustomTaxRate ?? 0.25m;
            var tax = subtotal * taxRate;
            var total = subtotal + tax + fee;

            existingEntity.Subtotal = subtotal;
            existingEntity.Tax = tax;
            existingEntity.Fee = fee;
            existingEntity.Total = total;
        }
    }
}
