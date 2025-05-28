using CarpetStore.Models;
using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public AdminController(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Dashboard()
        {
            var products = _productRepository.GetAllProducts().ToList();
            var orders = _orderRepository.GetAllOrders().ToList();
            return View((products, orders));
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = status;
            _orderRepository.UpdateOrder(order);
            return RedirectToAction(nameof(Dashboard));
        }
    }
} 