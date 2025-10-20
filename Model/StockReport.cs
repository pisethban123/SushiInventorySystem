using System;
using System.Linq;
using System.Collections.Generic;
using SushiInventorySystem.Data;
using SushiInventorySystem.Models;

namespace SushiInventorySystem.Models
{
    public class StockReport : IReport
    {
        private readonly List<Stock> _stocks;

        public StockReport(AppDbContext context)
        {
            // Pre-load necessary data
            _stocks = context.Stocks
                .Where(s => s.Item != null)
                .ToList();
        }

        public string GenerateSummary()
        {
            // Calculate in-memony data
            var totalStocks = _stocks.Count;
            var lowStocks = _stocks.Count(s => s.Item != null && s.Quantity < s.Item.MinStock);

            var avgCost = _stocks
                .Where(s => s.Item != null)
                .Average(s => s.Item!.CostPerUnit);

            return $"[Stock Report]\n" +
                   $"Total Stocks: {totalStocks}\n" +
                   $"Low Stock Items: {lowStocks}\n" +
                   $"Average Cost per Item: ${avgCost:F2}";
        }
    }
}
