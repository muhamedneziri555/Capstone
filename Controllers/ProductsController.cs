using CarpetStore.Models;
using CarpetStore.Models.Interfaces;
using CarpetStore.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private IProductRepository productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        [AllowAnonymous]
        public IActionResult Shop()
        {
            return View(productRepository.GetAllProducts());
        }
        [AllowAnonymous]
        public IActionResult Detail(int id)
        {
            var product = productRepository.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var products = productRepository.GetAllProducts();
            return View(products.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.AddProduct(product);
                return RedirectToAction("Index");
            }

            // If the model state is not valid, return to the create view with validation errors
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var product = productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound(); // Or handle appropriately (e.g., redirect to an error page)
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.UpdateProduct(product);
                return RedirectToAction("Index");
            }

            // If the model state is not valid, return to the edit view with validation errors
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound(); // Or handle appropriately (e.g., redirect to an error page)
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            productRepository.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }

}

