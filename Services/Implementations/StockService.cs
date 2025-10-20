﻿using SushiInventorySystem.Data;
using SushiInventorySystem.Models;
using SushiInventorySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SushiInventorySystem.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync() =>
            await _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .ToListAsync();

        public async Task<IEnumerable<Transfer>> GetTransferHistoryAsync() =>
            await _context.Transfers
                .Include(t => t.Item)
                .Include(t => t.From)
                .Include(t => t.To)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

        public async Task<Stock?> GetByIdAsync(string stockId) =>
            await _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(s => s.StockId == stockId);

        public async Task StockInAsync(string itemId, string branchId, int quantity)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ItemId == itemId && s.BranchId == branchId);

            if (stock == null)
            {
                stock = new Stock
                {
                    StockId = Guid.NewGuid().ToString(),
                    ItemId = itemId,
                    BranchId = branchId,
                    Quantity = quantity
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.Quantity += quantity;
                _context.Stocks.Update(stock);
            }

            await _context.SaveChangesAsync();
        }

        public async Task StockOutAsync(string itemId, string branchId, int quantity)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ItemId == itemId && s.BranchId == branchId);

            if (stock == null || stock.Quantity < quantity)
                throw new InvalidOperationException("Insufficient stock.");

            stock.Quantity -= quantity;
            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();
        }

        public async Task TransferAsync(string itemId, string fromBranch, string toBranch, int quantity, string unit)
        {
            // Start a transaction for full atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Validate stock first
                var fromStock = await _context.Stocks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ItemId == itemId && s.BranchId == fromBranch);

                if (fromStock == null || fromStock.Quantity < quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock in '{fromBranch}'. Available: {fromStock?.Quantity ?? 0}, Tried: {quantity}");
                }

                // 2️⃣ Perform stock operations
                await StockOutAsync(itemId, fromBranch, quantity);
                await StockInAsync(itemId, toBranch, quantity);

                // 3️⃣ Generate next TransferId (T0001, T0002...)
                var lastTransfer = await _context.Transfers
                    .AsNoTracking()
                    .OrderByDescending(t => t.TransferId)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (lastTransfer != null && lastTransfer.TransferId.StartsWith("T") &&
                    int.TryParse(lastTransfer.TransferId.Substring(1), out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }

                var transfer = new Transfer
                {
                    TransferId = $"T{nextNumber:D4}",
                    ItemId = itemId,
                    FromBranch = fromBranch,
                    ToBranch = toBranch,
                    Quantity = quantity,
                    Unit = unit,
                    TransferDate = DateTime.Now
                };

                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync();

                // 4️⃣ Commit transaction only if everything succeeded
                await transaction.CommitAsync();
            }
            catch
            {
                // ❌ Rollback on any error to maintain data consistency
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ==========================
        // Extended Features (LINQ + Grouping + Validation)
        // ==========================

        public async Task<IEnumerable<Stock>> GetLowStockItemsAsync()
        {
            return await _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Quantity < s.Item.MinStock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stock>> GetOverstockItemsAsync()
        {
            return await _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Quantity > s.Item.MaxStock)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetInventorySummaryAsync()
        {
            var stocks = await _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Branch != null)
                .ToListAsync();

            var summary = stocks
                .GroupBy(s => s.Branch!.BranchName)
                .Select(g => new
                {
                    Branch = g.Key,
                    TotalQty = g.Sum(x => x.Quantity),
                    TotalValue = g.Sum(x => x.Quantity * (x.Item?.CostPerUnit ?? 0))
                })
                .ToList<object>();

            return summary;
        }

    }
}
