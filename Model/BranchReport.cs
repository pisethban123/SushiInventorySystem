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
            // ✅ 데이터 즉시 메모리에 로드
            _branchCount = context.Branches.Count();

            _stocks = context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Branch != null)
                .ToList();
        }

        public string GenerateSummary()
        {
            // ✅ 메모리 내 데이터로 계산
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
