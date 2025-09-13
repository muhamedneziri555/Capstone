using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarpetStore.Models
{
    public class ProductSize
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string SizeName { get; set; } = string.Empty; // e.g., "120x170", "150x220", "Custom Size"
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; } // Optional description for custom sizes
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public Product? Product { get; set; }
    }
}

