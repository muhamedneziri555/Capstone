using System.ComponentModel.DataAnnotations;

namespace CarpetStore.Models
{
    public class ProductStock
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [StringLength(20)]
        public string Dimension { get; set; } = string.Empty; // "120x170", "150x220", "200x290"
        
        [Required]
        public int StockQuantity { get; set; }
        
        public int ReservedQuantity { get; set; } = 0; // Items in carts but not yet ordered
        
        public int AvailableStock => StockQuantity - ReservedQuantity;
        
        public bool IsInStock => AvailableStock > 0;
        
    }
}

