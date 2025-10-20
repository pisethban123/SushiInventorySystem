using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiInventorySystem.Models
{
    public class Stock
    {
        [Key]
        public string StockId { get; set; } = string.Empty;

        [Required]
        public string ItemId { get; set; } = string.Empty;

        [Required]
        public string BranchId { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }
}
