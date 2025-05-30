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

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository, IShoppingCartRepository shopCartRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _shopCartRepository = shopCartRepository;
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
                order.OrderDetails = new List<OrderDetail>();

                // Remove validation errors for system-generated fields
                ModelState.Remove("UserId");
                ModelState.Remove("OrderStatus");
                ModelState.Remove("OrderDetails");

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
            catch (Exception ex)
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
            return RedirectToAction("Index", "Orders");
        }
    }
}

