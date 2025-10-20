using SushiInventorySystem.Data;
using SushiInventorySystem.Models;
using SushiInventorySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SushiInventorySystem.Services.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _context;

        public BranchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetAllAsync() =>
            await _context.Branches.AsNoTracking().ToListAsync();

        public async Task<Branch?> GetByIdAsync(string id) =>
            await _context.Branches.FindAsync(id);

        public async Task CreateAsync(Branch branch)
        {
            if (string.IsNullOrWhiteSpace(branch.BranchId))
            {
                // Get the biggest BranchId (ex: B0008)
                var last = await _context.Branches
                    .OrderByDescending(b => b.BranchId)
                    .Select(b => b.BranchId)
                    .FirstOrDefaultAsync();

                // Calculate next number
                int nextNum = 1;
                if (!string.IsNullOrEmpty(last) && last.Length >= 5 && int.TryParse(last.Substring(1), out int num))
                    nextNum = num + 1;

                // Create new ID (ex: B0009)
                branch.BranchId = $"B{nextNum:D4}";
            }

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Branch branch)
        {
            var existingBranch = await _context.Branches.FindAsync(branch.BranchId);

            if (existingBranch != null)
            {
                existingBranch.BranchName = branch.BranchName;
                existingBranch.Address = branch.Address;
                existingBranch.Postcode = branch.Postcode;
                existingBranch.Phone = branch.Phone;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }
        }

        // ==========================
        // Extended Features (Anonymous Method + Validation)
        // ==========================

        public async Task<bool> ValidateBranchDataAsync(string branchId)
        {
            // Anonymous method (delegate)
            Func<string, bool> isValidId = delegate (string id)
            {
                return !string.IsNullOrEmpty(id) && id.StartsWith("B");
            };

            if (!isValidId(branchId))
                throw new ArgumentException("Invalid Branch ID format.");

            return await _context.Branches.AnyAsync(b => b.BranchId == branchId);
        }

    }
}
