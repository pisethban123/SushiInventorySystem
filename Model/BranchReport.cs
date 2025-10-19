using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SushiInventorySystem.Data;

namespace SushiInventorySystem.Models
{
    public class BranchReport : IReport
    {
        private readonly AppDbContext _context;
        public BranchReport(AppDbContext context) => _context = context;

        public string GenerateSummary()
        {
            var branchCount = _context.Branches.Count();

            // Read data from DB to memory and move to memory
            var stocks = _context.Stocks
                .Include(s => s.Item)
                .Include(s => s.Branch)
                .Where(s => s.Item != null && s.Branch != null)
                .ToList(); // Important: Load to Memory

            // Avg calculation in Memory by group
            var avgStock = stocks
                .GroupBy(s => s.Branch!.BranchName)
                .Select(g => g.Average(x => x.Quantity))
                .DefaultIfEmpty(0)
                .Average();

            return $"[Branch Report]\nTotal Branches: {branchCount}\nAverage Stock per Branch: {avgStock:F1}";
        }
    }
}
