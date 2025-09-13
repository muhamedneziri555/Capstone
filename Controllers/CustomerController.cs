using CarpetStore.Models;
using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public CustomerController(UserManager<ApplicationUser> userManager, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            var customers = _userManager.Users.ToList();
            return View(customers);
        }

        public async Task<IActionResult> Details(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Get customer's orders
            var orders = _orderRepository.GetOrdersByUserId(id).ToList();

            ViewBag.Orders = orders;
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _userManager.FindByIdAsync(model.Id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Update customer details
                customer.Name = model.Name;
                customer.Email = model.Email;
                customer.UserName = model.Email;
                customer.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(customer);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Customer details updated successfully!";
                    return RedirectToAction("Details", new { id = customer.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Check if customer has orders
            var orders = _orderRepository.GetOrdersByUserId(id);
            if (orders.Any())
            {
                TempData["Error"] = "Cannot delete customer with existing orders. Please delete orders first.";
                return RedirectToAction("Details", new { id = id });
            }

            var result = await _userManager.DeleteAsync(customer);
            if (result.Succeeded)
            {
                TempData["Success"] = "Customer deleted successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                TempData["Error"] = error.Description;
            }

            return RedirectToAction("Details", new { id = id });
        }
    }
}
