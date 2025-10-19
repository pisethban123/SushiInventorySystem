using System;
using System.Linq;
using SushiInventorySystem.Data;

namespace SushiInventorySystem.Models
{
    public class StockReport : IReport
    {
        private readonly AppDbContext _context;
        public StockReport(AppDbContext context) => _context = context;

        public string GenerateSummary()
        {
            var totalItems = _context.Stocks.Count();
            var lowStocks = _context.Stocks
                .Where(s => s.Item != null && s.Quantity < (s.Item.MinStock))
                .Count();

            return $"[Stock Report]\nTotal Stocks: {totalItems}\nLow Stock Items: {lowStocks}";
        }
    }
}
