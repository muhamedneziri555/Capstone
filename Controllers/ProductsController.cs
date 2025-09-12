using CarpetStore.Models;
using CarpetStore.Models.Interfaces;
using CarpetStore.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace CarpetStore.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private IProductRepository productRepository;
        private CarpetStore.Data.CarpetStoreWebDb _db;
        public ProductsController(IProductRepository productRepository, CarpetStore.Data.CarpetStoreWebDb db)
        {
            this.productRepository = productRepository;
            this._db = db;
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
            ViewBag.Categories = _db?.Categories?.OrderBy(c => c.Name).ToList() ?? new List<Category>();
            return View(products.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _db?.Categories?.OrderBy(c => c.Name).ToList() ?? new List<Category>();
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

            ViewBag.Categories = _db?.Categories?.OrderBy(c => c.Name).ToList() ?? new List<Category>();
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
        public IActionResult Category(string category, string? sortBy = "relevance", string? priceRange = "")
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return RedirectToAction("AllProducts");
            }

            var query = productRepository.GetAllProducts().AsQueryable();

            // Normalize category and derive leading name token (e.g., "Persian" from "Persian Collection")
            var normalizedCategory = (category ?? string.Empty).Trim();
            var leadingToken = normalizedCategory;
            var collectionSuffixIndex = normalizedCategory.IndexOf(" ");
            if (collectionSuffixIndex > 0)
            {
                leadingToken = normalizedCategory.Substring(0, collectionSuffixIndex);
            }
            var leadingLower = leadingToken.ToLower();

            // Filter by either exact Category field OR first-word prefix from Name
            query = query.Where(p =>
                (p.Category != null && p.Category == normalizedCategory) ||
                (p.Name != null && p.Name.ToLower().StartsWith(leadingLower))
            );

            // Price filtering
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                if (priceRange.Contains("-"))
                {
                    var parts = priceRange.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && decimal.TryParse(parts[0], out var min) && decimal.TryParse(parts[1], out var max))
                    {
                        query = query.Where(p => p.Price >= min && p.Price <= max);
                    }
                }
                else if (priceRange.EndsWith("+"))
                {
                    var number = priceRange.TrimEnd('+');
                    if (decimal.TryParse(number, out var minOnly))
                    {
                        query = query.Where(p => p.Price >= minOnly);
                    }
                }
            }

            // Sorting
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    break;
            }

            var products = query.ToList();

            ViewBag.Category = category;
            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.DebugInfo = $"Found {products.Count} products in {category}";
            ViewBag.AllCategories = new[] { "Acrylic Collection", "Persian Collection", "Polyester Collection", "Synthetic Collection", "Kids Collection" };

            return View(products);
        }

        [AllowAnonymous]
        public IActionResult DebugCategories()
        {
            var products = productRepository.GetAllProducts().ToList();
            return View(products);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(string? searchTerm, string? sortBy = "relevance", string? priceRange = "", string? category = "")
        {
            var query = productRepository.GetAllProducts().AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowered = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(lowered)) ||
                    (p.Detail != null && p.Detail.ToLower().Contains(lowered))
                );
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category == category);
            }

            // Filter by price range
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                if (priceRange.Contains("-"))
                {
                    var parts = priceRange.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && decimal.TryParse(parts[0], out var min) && decimal.TryParse(parts[1], out var max))
                    {
                        query = query.Where(p => p.Price >= min && p.Price <= max);
                    }
                }
                else if (priceRange.EndsWith("+"))
                {
                    var number = priceRange.TrimEnd('+');
                    if (decimal.TryParse(number, out var minOnly))
                    {
                        query = query.Where(p => p.Price >= minOnly);
                    }
                }
            }

            // Sorting
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    // relevance or unknown -> keep current ordering
                    break;
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.Category = category;

            var results = query.ToList();
            return View(results);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Category name is required.";
                return RedirectToAction("Index");
            }

            if (_db.Categories.Any(c => c.Name == name))
            {
                TempData["Error"] = "Category already exists.";
                return RedirectToAction("Index");
            }

            _db.Categories.Add(new Category { Name = name });
            _db.SaveChanges();
            TempData["Success"] = "Category created.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Index");
            }

            // Check if any products are using this category
            var productsUsingCategory = _db.Products.Any(p => p.Category == category.Name);
            if (productsUsingCategory)
            {
                TempData["Error"] = $"Cannot delete '{category.Name}' category. It is being used by products.";
                return RedirectToAction("Index");
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["Success"] = $"Category '{category.Name}' deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}

