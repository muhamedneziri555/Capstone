using System.ComponentModel.DataAnnotations;

namespace CarpetStore.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int ProductId { get; set; }
        
        public Product Product { get; set; } = null!;
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}








