﻿using DigitalWallet.Application.Models;
using DigitalWallet.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalWallet.Application.Repositories.Bank
{
    public interface IBankRepository
    {
        /// <summary>
        /// Create a new bank for user.
        /// </summary>
        /// <param name="bank">New bank model object.</param>
        Task<Result> CreateAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Get all banks as paginated.
        /// </summary>
        /// <param name="page">Number of page.</param>
        /// <param name="pageSize">Current page items count.</param>
        /// <param name="keyword">Keyword for search in bank name.</param>
        Task<PaginatedList<BankEntity>> GetAllAsync(int page = 1, int pageSize = 10, string keyword = null,
            CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Find a specific bank of a user.
        /// </summary>
        /// <param name="id"></param>
        Task<BankEntity> FindByIdAsync(Guid id, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Update a specific bank.
        /// </summary>
        /// <param name="bank">Bank object model to update.</param>
        Task<Result> UpdateAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Delete a specific bank.
        /// </summary>
        /// <param name="bank">Bank object model to delete.</param>
        Task<Result> DeleteAsync(BankEntity bank, CancellationToken cancellationToken = new CancellationToken());
    }
}