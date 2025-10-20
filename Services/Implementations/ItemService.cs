using SushiInventorySystem.Data;
using SushiInventorySystem.Models;
using SushiInventorySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SushiInventorySystem.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllAsync() =>
            await _context.Items.AsNoTracking().ToListAsync();

        public async Task<Item?> GetByIdAsync(string id) =>
            await _context.Items.FindAsync(id);
        public async Task CreateAsync(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.ItemId))
            {
                var last = await _context.Items
                    .OrderByDescending(i => i.ItemId)
                    .Select(i => i.ItemId)
                    .FirstOrDefaultAsync();

                int nextNum = 1;
                if (!string.IsNullOrEmpty(last) && last.Length >= 5 && int.TryParse(last.Substring(1), out int num))
                    nextNum = num + 1;

                item.ItemId = $"I{nextNum:D4}";
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Item item)
        {
            var existingItem = await _context.Items.FindAsync(item.ItemId);

            if (existingItem != null)
            {
                existingItem.ItemName = item.ItemName;
                existingItem.Category = item.Category;
                existingItem.Unit = item.Unit;
                existingItem.Supplier = item.Supplier;
                existingItem.CostPerUnit = item.CostPerUnit;
                existingItem.MinStock = item.MinStock;
                existingItem.MaxStock = item.MaxStock;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // ==========================
        // Extended Features (LINQ + Lambda + Anonymous)
        // ==========================

        public async Task<IEnumerable<Item>> SearchItemsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty.");

            return await _context.Items
                .Where(i => i.ItemName.ToLower().Contains(keyword.ToLower()) ||
                            i.Category.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetAverageCostByCategoryAsync()
        {
            var result = await _context.Items
                .GroupBy(i => i.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    AvgCost = g.Average(x => x.CostPerUnit)
                })
                .ToListAsync();

            // Anonymous → Dictionary Conversion
            return result.ToDictionary(x => x.Category, x => x.AvgCost);
        }

        public async Task<IEnumerable<Item>> GetExpensiveItemsAsync(decimal threshold)
        {
            return await _context.Items
                .Where(i => i.CostPerUnit >= threshold)
                .OrderByDescending(i => i.CostPerUnit)
                .ToListAsync();
        }

    }
}
