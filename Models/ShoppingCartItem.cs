namespace CarpetStore.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Product? Product { get; set; }
        public int Qty { get; set; }
        public string? ShoppingCartId { get; set; }
        public string? SelectedSize { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}

