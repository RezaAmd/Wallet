﻿using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces.Context;
using DigitalWallet.Application.Models;
using DigitalWallet.Domain.Entities;
using System.Data.Entity;

namespace DigitalWallet.Application.Repositories.Bank
{
    public class BankRepository : IBankRepository
    {
        #region Initialize
        private readonly IDbContext context;
        public BankRepository(IDbContext _context)
        {
            context = _context;
        }
        #endregion

        /// <summary>
        /// Create a new bank for user.
        /// </summary>
        /// <param name="bank">New bank model object.</param>
        public async Task<Result> CreateAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken())
        {
            await context.Banks.AddAsync(bank);
            if (Convert.ToBoolean(await context.SaveChangesAsync(cancellationToken)))
                return Result.Success;
            return Result.Failed();
        }

        /// <summary>
        /// Get all banks as paginated.
        /// </summary>
        /// <param name="userId">Filter for for specific owner.</param>
        /// <param name="page">Number of page.</param>
        /// <param name="pageSize">Current page items count.</param>
        /// <param name="keyword">Keyword for search in bank name.</param>
        public async Task<PaginatedList<BankEntity>> GetAllAsync(int page = 1, int pageSize = 10, string keyword = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var banks = context.Banks.AsQueryable();
            #region Filters
            if (!string.IsNullOrEmpty(keyword))
                banks = banks.Where(b => keyword.Contains(b.Name));
            #endregion
            return await banks.PaginatedListAsync(page, pageSize, cancellationToken);
        }

        /// <summary>
        /// Find a specific bank of a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BankEntity> FindByIdAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var banks = context.Banks.Where(b => b.Id == id);
            return await banks.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Update a specific bank.
        /// </summary>
        /// <param name="bank">Bank object model to update.</param>
        public async Task<Result> UpdateAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken())
        {
            context.Banks.Update(bank);
            if (Convert.ToBoolean(await context.SaveChangesAsync(cancellationToken)))
                return Result.Success;
            return Result.Failed();
        }

        /// <summary>
        /// Delete a specific bank.
        /// </summary>
        /// <param name="bank">Bank object model to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken())
        {
            context.Banks.Remove(bank);
            if (Convert.ToBoolean(await context.SaveChangesAsync(cancellationToken)))
                return Result.Success;
            return Result.Failed();
        }
    }
}