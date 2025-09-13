using CarpetStore.Models.Interfaces;
using CarpetStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarpetStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shopCartRepository;
        private readonly IProductStockRepository _stockRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository, IShoppingCartRepository shopCartRepository, IProductStockRepository stockRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _shopCartRepository = shopCartRepository;
            _stockRepository = stockRepository;
        }

        public IActionResult Checkout()
        {
            var cartItems = _shopCartRepository.GetShoppingCartItems();
            var total = _shopCartRepository.GetShoppingCartTotal();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "ShoppingCart");
            }

            ViewBag.CartItems = cartItems;
            ViewBag.Total = total;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            try
            {
                // Get the current user's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "User not authenticated");
                    return View(order);
                }

                // Set system-generated fields before validation
                order.UserId = userId;
                order.OrderDate = DateTime.Now;
                order.OrderStatus = "Pending";
                order.PaymentStatus = "Pending";
                order.OrderDetails = new List<OrderDetail>();

                // Remove validation errors for system-generated fields
                ModelState.Remove("UserId");
                ModelState.Remove("OrderStatus");
                ModelState.Remove("OrderDetails");
                
                // Remove validation errors for card fields if payment method doesn't require them
                if (order.PaymentMethod != "Credit Card" && order.PaymentMethod != "Debit Card")
                {
                    ModelState.Remove("CardNumber");
                    ModelState.Remove("CardHolderName");
                    ModelState.Remove("ExpiryDate");
                    ModelState.Remove("CVV");
                }

                if (!ModelState.IsValid)
                {
                    var cartItems = _shopCartRepository.GetShoppingCartItems();
                    var total = _shopCartRepository.GetShoppingCartTotal();

                    ViewBag.CartItems = cartItems;
                    ViewBag.Total = total;
                    return View(order);
                }

                var items = _shopCartRepository.GetShoppingCartItems();
                if (!items.Any())
                {
                    ModelState.AddModelError("", "Your cart is empty");
                    return View(order);
                }

                // Place the order
                _orderRepository.PlaceOrder(order);

                // Consume reserved stock for each item in the order
                foreach (var item in items)
                {
                    _stockRepository.ConsumeStock(item.Product.Id, item.SelectedSize, item.Qty);
                }

                // Clear the cart
                _shopCartRepository.ClearCart();
                HttpContext.Session.SetInt32("CartCount", 0);

                // Redirect to My Orders
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                // Log the error
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                
                // Return to checkout with the same data
                var cartItems = _shopCartRepository.GetShoppingCartItems();
                var total = _shopCartRepository.GetShoppingCartTotal();

                ViewBag.CartItems = cartItems;
                ViewBag.Total = total;
                return View(order);
            }
        }

        public IActionResult CheckoutComplete()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult MyOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var orders = _orderRepository.GetOrdersByUserId(userId);
                return View(orders);
            }
            catch (Exception)
            {
                // Log the error
                TempData["Error"] = "An error occurred while retrieving your orders";
                return View(new List<Order>());
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = status;
            _orderRepository.UpdateOrder(order);
            return RedirectToAction("Index", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOrder(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            // Restore stock for each item in the order
            foreach (var item in order.OrderDetails)
            {
                if (!string.IsNullOrEmpty(item.Size))
                {
                    _stockRepository.ReleaseStock(item.ProductId, item.Size, item.Quantity);
                }
            }

            // Delete the order
            _orderRepository.DeleteOrder(id);
            
            TempData["Success"] = "Order deleted successfully!";
            return RedirectToAction("Index", "Admin");
        }
    }
}

