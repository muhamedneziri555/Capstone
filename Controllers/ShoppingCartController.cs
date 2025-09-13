using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private IShoppingCartRepository shoppingCartRepository;
        private IProductRepository productRepository;
        private IProductStockRepository stockRepository;
        
        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository, IProductStockRepository stockRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.stockRepository = stockRepository;
        }
        public IActionResult Index()
        {
            var items = shoppingCartRepository.GetShoppingCartItems();
            shoppingCartRepository.ShoppingCartItems = items;
            ViewBag.CartTotal = shoppingCartRepository.GetShoppingCartTotal();
            return View(items);
        }


        public RedirectToActionResult AddToShoppingCart(int pId, string selectedSize = "120x170")
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pId);
            if (product != null)
            {
                // Check if product is in stock for the selected dimension
                if (!stockRepository.IsInStock(pId, selectedSize, 1))
                {
                    TempData["Error"] = $"Sorry, this product is currently out of stock in size {selectedSize}";
                    return RedirectToAction("Detail", "Products", new { id = pId });
                }

                // Determine the price based on selected size
                decimal unitPrice = selectedSize switch
                {
                    "120x170" => product.Price120x170 ?? product.Price,
                    "150x220" => product.Price150x220 ?? product.Price,
                    "200x290" => product.Price200x290 ?? product.Price,
                    _ => product.Price
                };
                
                // Reserve stock when adding to cart
                try
                {
                    stockRepository.ReserveStock(pId, selectedSize, 1);
                    shoppingCartRepository.AddToCart(product, selectedSize, unitPrice);
                    int cartCount = shoppingCartRepository.GetShoppingCartItems().Count;
                    HttpContext.Session.SetInt32("CartCount", cartCount);
                    TempData["Success"] = "Product added to cart successfully!";
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction("Detail", "Products", new { id = pId });
                }
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int pId)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pId);
            if (product != null)
            {
                // Get the cart item to find the selected size
                var cartItems = shoppingCartRepository.GetShoppingCartItems();
                var cartItem = cartItems.FirstOrDefault(ci => ci.Product.Id == pId);
                
                if (cartItem != null)
                {
                    // Release reserved stock when removing from cart
                    stockRepository.ReleaseStock(pId, cartItem.SelectedSize, 1);
                }
                
                shoppingCartRepository.RemoveFromCart(product);
                int cartCount = shoppingCartRepository.GetShoppingCartItems().Count;
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }

    }
}

