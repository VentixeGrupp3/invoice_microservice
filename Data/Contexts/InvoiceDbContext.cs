using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts
{
    public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options)
    {
        public DbSet<InvoiceEntity> Invoices { get; set; } = null!;
        public DbSet<InvoiceItemEntity> InvoiceItems { get; set; } = null!;

    }
}
