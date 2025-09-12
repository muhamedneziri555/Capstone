using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
	public class ShoppingCartRepository : IShoppingCartRepository
	{
		private CarpetStoreWebDb dbContext;
		public ShoppingCartRepository(CarpetStoreWebDb dbContext)
		{
			this.dbContext = dbContext;
		}
		public List<ShoppingCartItem>? ShoppingCartItems { get; set; }
		public string? ShoppingCartId { get; set; }
		public static ShoppingCartRepository GetCart(IServiceProvider services)
		{
			ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

			CarpetStoreWebDb context = services.GetService<CarpetStoreWebDb>() ?? throw new Exception("Error initializing carpetstoredb");

			string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();

			session?.SetString("CartId", cartId);

			return new ShoppingCartRepository(context) { ShoppingCartId = cartId };
		}


		public void AddToCart(Product product, string selectedSize, decimal unitPrice)
		{
			var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId && s.SelectedSize == selectedSize);
			if (shoppingCartItem == null)
			{
				shoppingCartItem = new ShoppingCartItem
				{
					ShoppingCartId = ShoppingCartId,
					Product = product,
					Qty = 1,
					SelectedSize = selectedSize,
					UnitPrice = unitPrice
				};
				dbContext.ShoppingCartItems.Add(shoppingCartItem);
			}
			else
			{
				shoppingCartItem.Qty++;
				shoppingCartItem.UnitPrice = unitPrice;
			}
			dbContext.SaveChanges();
		}

		public void ClearCart()
		{
			var cartItems = dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId);
			dbContext.ShoppingCartItems.RemoveRange(cartItems);
			dbContext.SaveChanges();
		}

		public List<ShoppingCartItem> GetShoppingCartItems()
		{
			return ShoppingCartItems ??= dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId)
				   .Include(p => p.Product).ToList();
		}

		public decimal GetShoppingCartTotal()
		{
			var totalCost = dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId)
				  .Select(s => s.UnitPrice * s.Qty).Sum();
			return totalCost;
		}

		public int RemoveFromCart(Product product, string? selectedSize = null)
		{
			{
				var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId && (selectedSize == null || s.SelectedSize == selectedSize));
				var quantity = 0;

				if (shoppingCartItem != null)
				{
					if (shoppingCartItem.Qty > 1)
					{
						shoppingCartItem.Qty--;
						quantity = shoppingCartItem.Qty;
					}
					else
					{
						dbContext.ShoppingCartItems.Remove(shoppingCartItem);
					}
				}
				dbContext.SaveChanges();
				return quantity;
			}
		}
	}
}

