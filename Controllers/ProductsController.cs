using CarpetStore.Models;
using CarpetStore.Models.Interfaces;
using CarpetStore.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        [AllowAnonymous]
        public IActionResult AllProducts()
        {
            var products = productRepository.GetAllProducts().ToList();
            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Category(string category, string sortBy, string priceRange)
        {
            if (string.IsNullOrEmpty(category))
            {
                return NotFound();
            }

            var products = productRepository.GetAllProducts()
                .Where(p => p.Name != null && p.Name.Split(' ')[0].Equals(category, StringComparison.OrdinalIgnoreCase))
                .AsQueryable();

            // Apply price range filter
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                switch (priceRange)
                {
                    case "0-100":
                        products = products.Where(p => p.Price <= 100);
                        break;
                    case "100-500":
                        products = products.Where(p => p.Price > 100 && p.Price <= 500);
                        break;
                    case "500-1000":
                        products = products.Where(p => p.Price > 500 && p.Price <= 1000);
                        break;
                    case "1000+":
                        products = products.Where(p => p.Price > 1000);
                        break;
                }
            }

            // Apply sorting
            products = sortBy switch
            {
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "name_asc" => products.OrderBy(p => p.Name),
                "name_desc" => products.OrderByDescending(p => p.Name),
                _ => products // Default to relevance (no specific sorting)
            };

            ViewBag.Category = category;
            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.DebugInfo = $"Found {products.Count()} products in {category} Collection";

            return View(products.ToList());
        }

        [AllowAnonymous]
        public IActionResult DebugCategories()
        {
            var products = productRepository.GetAllProducts().ToList();
            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Search(string searchTerm, string sortBy, string priceRange, string category)
        {
            var products = productRepository.GetAllProducts().AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                products = products.Where(p => 
                    p.Name.ToLower().Contains(searchTermLower) || 
                    p.Detail.ToLower().Contains(searchTermLower));
            }

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                // Extract the first word from the category (e.g., "Persian" from "Persian Collection")
                var categoryFirstWord = category.Split(' ')[0].ToLower();
                products = products.Where(p => p.Name.ToLower().StartsWith(categoryFirstWord));
            }

            // Apply price range filter
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                switch (priceRange)
                {
                    case "0-100":
                        products = products.Where(p => p.Price <= 100);
                        break;
                    case "100-500":
                        products = products.Where(p => p.Price > 100 && p.Price <= 500);
                        break;
                    case "500-1000":
                        products = products.Where(p => p.Price > 500 && p.Price <= 1000);
                        break;
                    case "1000+":
                        products = products.Where(p => p.Price > 1000);
                        break;
                }
            }

            // Apply sorting
            products = sortBy switch
            {
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "name_asc" => products.OrderBy(p => p.Name),
                "name_desc" => products.OrderByDescending(p => p.Name),
                _ => products // Default to relevance (no specific sorting)
            };

            // Store filter values in ViewBag for maintaining state
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.Category = category;

            return View(products.ToList());
        }
    }

}

