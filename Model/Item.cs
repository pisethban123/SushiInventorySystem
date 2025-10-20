using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SushiInventorySystem.Models
{
    public class Item
    {
        [Key]
        public string ItemId { get; set; } = string.Empty;

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string Supplier { get; set; } = string.Empty;

        public decimal CostPerUnit { get; set; }

        public int MinStock { get; set; }

        public int MaxStock { get; set; }

        // Navigation
        public ICollection<Stock>? Stocks { get; set; }
        public ICollection<Transfer>? Transfers { get; set; }
    }
}
