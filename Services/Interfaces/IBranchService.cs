using SushiInventorySystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SushiInventorySystem.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch?> GetByIdAsync(string id);
        Task CreateAsync(Branch branch);
        Task UpdateAsync(Branch branch);
        Task DeleteAsync(string id);

        // (Validation + Anonymous method)
        Task<bool> ValidateBranchDataAsync(string branchId);
    }
}
