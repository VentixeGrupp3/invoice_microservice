using Business.Forms;
using Data.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories
{
    public class SoftDeleteInvoiceMappingFactory : IUpdateMappingFactory<InvoiceEntity, SoftDeleteInvoiceForm>
    {
        public void MapFormToExistingEntity(SoftDeleteInvoiceForm form, InvoiceEntity existingEntity)
        {
            existingEntity.IsDeleted = true;
            existingEntity.DeletionReason = form.DeletionReason;
            existingEntity.DeletedAt = DateTime.Now;
            existingEntity.DeletedBy = form.DeletedBy;
        }

        
    }
}
