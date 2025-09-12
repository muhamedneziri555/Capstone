namespace CarpetStore.Models.Interfaces
{
	public interface IShoppingCartRepository
	{
		void AddToCart(Product product, string selectedSize, decimal unitPrice);
		int RemoveFromCart(Product product, string? selectedSize = null);
		List<ShoppingCartItem> GetShoppingCartItems();
		void ClearCart();
		decimal GetShoppingCartTotal();
		public List<ShoppingCartItem>? ShoppingCartItems { get; set; }
	}
}
