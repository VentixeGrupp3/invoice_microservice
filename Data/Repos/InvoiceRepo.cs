﻿using System;
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
    // InvoiceItems are value-like children that shouldn’t be managed outside their parent Invoice.
    // All creation, updates, and deletions should happen through the Invoice aggregate.

    public interface IInvoiceRepo
    {
        Task AddAsync(InvoiceEntity entity);
        Task UpdateAsync(InvoiceEntity entity);
        Task HardDeleteAsync(string invoiceId);
        Task<bool> ExistsAsync(Expression<Func<InvoiceEntity, bool>> predicate, bool includeDeleted = false);

        /// <summary>
        /// Retrieves a single invoice matching the given filter condition.
        /// Includes any specified related navigation properties (e.g., InvoiceItems).
        /// </summary>
        Task<InvoiceEntity?> GetAsync(
            Expression<Func<InvoiceEntity, bool>> where,
            params Expression<Func<InvoiceEntity, object>>[] includes);

        /// <summary>
        /// Retrieves a single invoice including soft-deleted ones, matching the given filter condition.
        /// Used for administrative or audit purposes.
        /// </summary>
        Task<InvoiceEntity?> GetIncludingDeletedAsync(
            Expression<Func<InvoiceEntity, bool>> where,
            params Expression<Func<InvoiceEntity, object>>[] includes);

        /// <summary>
        /// Retrieves all invoices matching optional filter/sort conditions,
        /// including any navigation properties.
        /// </summary>
        Task<IEnumerable<InvoiceEntity>> GetAllAsync(
            Expression<Func<InvoiceEntity, bool>>? where = null,
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes);

        /// <summary>
        /// Retrieves invoices as a projected shape (TSelect), useful for DTOs or summaries.
        /// Includes filtering, sorting, and eager loading support.
        /// </summary>
        Task<IEnumerable<TSelect>> GetAllAsync<TSelect>(
            Expression<Func<InvoiceEntity, TSelect>> selector,
            Expression<Func<InvoiceEntity, bool>>? where = null,
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes);

        /// <summary>
        /// Retrieves all invoices including soft-deleted ones.
        /// Used for administrative or audit purposes.
        /// Supports sorting and eager loading of navigation properties.
        /// </summary>
        Task<IEnumerable<InvoiceEntity>> GetAllIncludingDeletedAsync(
            Expression<Func<InvoiceEntity, object>>? sortBy = null,
            bool orderByDescending = false,
            params Expression<Func<InvoiceEntity, object>>[] includes);
    }

    public class InvoiceRepo(InvoiceDbContext context) : IInvoiceRepo
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

        public async Task HardDeleteAsync(string invoiceId)
        {
            var invoice = await _context.Invoices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice is not null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<InvoiceEntity, bool>> predicate, bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Invoices.IgnoreQueryFilters()
                : _context.Invoices;

            return await query.AnyAsync(predicate);
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
