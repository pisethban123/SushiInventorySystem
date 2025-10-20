using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiInventorySystem.Models
{
    public class Transfer
    {
        [Key]
        public string TransferId { get; set; } = string.Empty;

        [Required]
        public string ItemId { get; set; } = string.Empty;

        public double Quantity { get; set; }

        public string Unit { get; set; } = string.Empty;

        [Required]
        public string FromBranch { get; set; } = string.Empty;

        [Required]
        public string ToBranch { get; set; } = string.Empty;

        public DateTime TransferDate { get; set; }

        // Navigation
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(FromBranch))]
        public Branch? From { get; set; }

        [ForeignKey(nameof(ToBranch))]
        public Branch? To { get; set; }
    }
}
