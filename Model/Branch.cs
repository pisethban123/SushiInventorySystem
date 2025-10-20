using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SushiInventorySystem.Models
{
    public class Branch
    {
        [Key]
        public string BranchId { get; set; } = string.Empty;

        [Required]
        public string BranchName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        // Navigation
        public ICollection<Stock>? Stocks { get; set; }
        public ICollection<Transfer>? TransfersFrom { get; set; }
        public ICollection<Transfer>? TransfersTo { get; set; }
    }
}
