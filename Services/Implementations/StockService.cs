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
            await StockOutAsync(itemId, fromBranch, quantity);
            await StockInAsync(itemId, toBranch, quantity);

            var transfer = new Transfer
            {
                TransferId = Guid.NewGuid().ToString(),
                ItemId = itemId,
                Quantity = quantity,
                Unit = unit,
                FromBranch = fromBranch,
                ToBranch = toBranch,
                TransferDate = DateTime.Now
            };

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();
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
