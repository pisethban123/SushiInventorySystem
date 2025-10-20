using SushiInventorySystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SushiInventorySystem.Services.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task<IEnumerable<Transfer>> GetTransferHistoryAsync();
        Task<Stock?> GetByIdAsync(string stockId);
        Task StockInAsync(string itemId, string branchId, int quantity);
        Task StockOutAsync(string itemId, string branchId, int quantity);
        Task TransferAsync(string itemId, string fromBranch, string toBranch, int quantity, string unit);

        // (LINQ + GroupBy + Validation)
        Task<IEnumerable<Stock>> GetLowStockItemsAsync();
        Task<IEnumerable<Stock>> GetOverstockItemsAsync();
        Task<IEnumerable<object>> GetInventorySummaryAsync();
    }
}
