using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SushiInventorySystem.Data;
using System.Collections.Generic;
using SushiInventorySystem.Models;

namespace SushiInventorySystem.Models
{
    public class BranchReport : IReport
    {
        private readonly int _branchCount;
        private readonly List<Stock> _stocks;

        public BranchReport(AppDbContext context)
        {
            // Load data to the memory
            _branchCount = context.Branches.Count();

            _stocks = context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Branch != null)
                .ToList();
        }

        public string GenerateSummary()
        {
            // Calculate data in memory
            var avgStockByBranch = _stocks
                .GroupBy(s => s.Branch!.BranchName)
                .Select(g => new
                {
                    Branch = g.Key,
                    AvgQty = g.Average(x => x.Quantity)
                })
                .ToList();

            var avgOverall = avgStockByBranch.Select(a => a.AvgQty).DefaultIfEmpty(0).Average();

            return $"[Branch Report]\n" +
                   $"Total Branches: {_branchCount}\n" +
                   $"Average Stock per Branch: {avgOverall:F1}";
        }
    }
}
