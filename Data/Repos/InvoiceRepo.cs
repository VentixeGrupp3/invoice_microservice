using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repos
{
    public class InvoiceRepo(InvoiceDbContext context)
    {
        protected readonly InvoiceDbContext _context = context;

        public async Task AddAsync(InvoiceEntity entity)
        {
            await _context.Invoices.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InvoiceEntity entity)
        {
            _context.Invoices.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(InvoiceEntity entity)
        {
            _context.Invoices.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(string invoiceId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice is not null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string invoiceId)
        {
            return await _context.Invoices.AnyAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<bool> ExistsIncludingDeletedAsync(string invoiceId)
        {
            return await _context.Invoices
                .IgnoreQueryFilters()
                .AnyAsync(i => i.InvoiceId == invoiceId);
        }


        /// <summary>
        /// Retrieves a single invoice matching the given filter condition.
        /// Includes any specified related navigation properties (e.g., InvoiceItems).
        /// </summary>
        public async Task<InvoiceEntity?> GetAsync(
            Expression<Func<InvoiceEntity, bool>> where,
            params Expression<Func<InvoiceEntity, object>>[] includes)
        {
            IQueryable<InvoiceEntity> query = _context.Invoices;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// Retrieves a single invoice including soft-deleted ones, matching the given filter condition.
        /// Used for administrative or audit purposes.
        /// </summary>
        public async Task<InvoiceEntity?> GetIncludingDeletedAsync(
            Expression<Func<InvoiceEntity, bool>> where,
            params Expression<Func<InvoiceEntity, object>>[] includes)
        {
            IQueryable<InvoiceEntity> query = _context.Invoices.IgnoreQueryFilters();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// Retrieves all invoices matching optional filter/sort conditions,
        /// including any navigation properties.
        /// </summary>
        public async Task<IEnumerable<InvoiceEntity>> GetAllAsync(
            Expression<Func<InvoiceEntity, bool>>? where = null,
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes)
        {
            IQueryable<InvoiceEntity> query = _context.Invoices;

            if (where != null)
                query = query.Where(where);

            foreach (var include in includes)
                query = query.Include(include);

            if (sortBy != null)
            {
                query = orderByDescending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves invoices as a projected shape (TSelect), useful for DTOs or summaries.
        /// Includes filtering, sorting, and eager loading support.
        /// </summary>
        public async Task<IEnumerable<TSelect>> GetAllAsync<TSelect>(
            Expression<Func<InvoiceEntity, TSelect>> selector,
            Expression<Func<InvoiceEntity, bool>>? where = null,
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes)
        {
            IQueryable<InvoiceEntity> query = _context.Invoices;

            if (where != null)
                query = query.Where(where);

            foreach (var include in includes)
                query = query.Include(include);

            if (sortBy != null)
            {
                query = orderByDescending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);
            }

            return await query.Select(selector).ToListAsync();
        }
        /// <summary>
        /// Retrieves all invoices including soft-deleted ones.
        /// Used for administrative or audit purposes.
        /// Supports sorting and eager loading of navigation properties.
        /// </summary>
        public async Task<IEnumerable<InvoiceEntity>> GetAllIncludingDeletedAsync(
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes)
        {
            IQueryable<InvoiceEntity> query = _context.Invoices.IgnoreQueryFilters();

            foreach (var include in includes)
                query = query.Include(include);

            if (sortBy != null)
            {
                query = orderByDescending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);
            }

            return await query.ToListAsync();
        }

    }
}
