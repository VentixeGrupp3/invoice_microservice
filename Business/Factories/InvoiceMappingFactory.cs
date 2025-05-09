using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Models;
using Data.Entities;

namespace Business.Factories
{
    public class InvoiceMappingFactory : IMappingFactory<InvoiceEntity, InvoiceModel>
    {
        public InvoiceEntity MapToEntity(InvoiceModel model)
        {
            return new InvoiceEntity
            {
                BookingId = model.BookingId,
                EventId = model.EventId,
                InvoicePaid = model.InvoicePaid,
                IssuedDate = model.IssuedDate,
                DueDate = model.DueDate,
                Created = model.Created == default ? DateTime.UtcNow : model.Created,
                Subtotal = model.Subtotal,
                Tax = model.Tax,
                Fee = model.Fee,
                Total = model.Total,

                EventOwnerName = model.EventOwnerName,
                EventOwnerEmail = model.EventOwnerEmail,
                EventOwnerAddress = model.EventOwnerAddress,
                EventOwnerPhone = model.EventOwnerPhone,

                UserName = model.UserName,
                UserEmail = model.UserEmail,
                UserAddress = model.UserAddress,
                UserPhone = model.UserPhone,

                InvoiceItems = model.Items.Select(i => new InvoiceItemEntity
                {
                    TicketCategory = i.TicketCategory,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public InvoiceModel MapToModel(InvoiceEntity entity)
        {
            return new InvoiceModel
            {
                InvoiceId = entity.InvoiceId,
                BookingId = entity.BookingId,
                EventId = entity.EventId,
                InvoicePaid = entity.InvoicePaid,
                IssuedDate = entity.IssuedDate,
                DueDate = entity.DueDate,
                Created = entity.Created,
                Subtotal = entity.Subtotal,
                Tax = entity.Tax,
                Fee = entity.Fee,
                Total = entity.Total,

                EventOwnerName = entity.EventOwnerName,
                EventOwnerEmail = entity.EventOwnerEmail,
                EventOwnerAddress = entity.EventOwnerAddress,
                EventOwnerPhone = entity.EventOwnerPhone,

                UserName = entity.UserName,
                UserEmail = entity.UserEmail,
                UserAddress = entity.UserAddress,
                UserPhone = entity.UserPhone,

                Items = entity.InvoiceItems.Select(i => new InvoiceItemModel
                {
                    Id = i.Id,
                    TicketCategory = i.TicketCategory,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
