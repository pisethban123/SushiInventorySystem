using SushiInventorySystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SushiInventorySystem.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(string id);
        Task CreateAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(string id);

        // (LINQ + Generics + Lambda)
        Task<IEnumerable<Item>> SearchItemsAsync(string keyword);
        Task<Dictionary<string, decimal>> GetAverageCostByCategoryAsync();
        Task<IEnumerable<Item>> GetExpensiveItemsAsync(decimal threshold);
    }
}
