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

            // Load custom sizes for this product
            List<ProductSize> customSizes = new List<ProductSize>();
            try
            {
                customSizes = _db?.ProductSizes?
                    .Where(ps => ps.ProductId == id && ps.IsActive)
                    .OrderBy(ps => ps.SizeName)
                    .ToList() ?? new List<ProductSize>();
            }
            catch
            {
                // Table doesn't exist yet, return empty list
                customSizes = new List<ProductSize>();
            }

            ViewBag.Categories = _db?.Categories?.OrderBy(c => c.Name).ToList() ?? new List<Category>();
            ViewBag.CustomSizes = customSizes;
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
        public IActionResult AllProducts(string? sortBy = "relevance", string? priceRange = "", string? category = "", string? size = "", int page = 1, int pageSize = 12)
        {
            var query = productRepository.GetAllProducts().AsQueryable();

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category == category);
            }

            // Filter by size (using size-specific pricing and custom sizes)
            if (!string.IsNullOrWhiteSpace(size))
            {
                switch (size.ToLower())
                {
                    case "120x170":
                        query = query.Where(p => p.Price120x170.HasValue);
                        break;
                    case "150x220":
                        query = query.Where(p => p.Price150x220.HasValue);
                        break;
                    case "200x290":
                        query = query.Where(p => p.Price200x290.HasValue);
                        break;
                    default:
                        // Check for custom sizes
                        query = query.Where(p => p.CustomSizes != null && p.CustomSizes.Any(cs => cs.SizeName.ToLower() == size.ToLower() && cs.IsActive));
                        break;
                }
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
                    break;
            }

            var totalCount = query.Count();
            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.Category = category;
            ViewBag.Size = size;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.AllCategories = _db?.Categories?.OrderBy(c => c.Name).Select(c => c.Name).ToList() ?? new List<string>();
            ViewBag.AllSizes = GetAllAvailableSizes();
            ViewBag.ResultsCount = products.Count;

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
            ViewBag.AllCategories = _db?.Categories?.OrderBy(c => c.Name).Select(c => c.Name).ToList() ?? new List<string>();

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
        public IActionResult Search(string? searchTerm, string? sortBy = "relevance", string? priceRange = "", string? category = "", string? size = "", int page = 1, int pageSize = 12)
        {
            var query = productRepository.GetAllProducts().AsQueryable();

            // Basic search (Entity Framework compatible)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowered = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(lowered)) ||
                    (p.Detail != null && p.Detail.ToLower().Contains(lowered)) ||
                    (p.Category != null && p.Category.ToLower().Contains(lowered))
                );
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category == category);
            }

            // Filter by size (using size-specific pricing and custom sizes)
            if (!string.IsNullOrWhiteSpace(size))
            {
                switch (size.ToLower())
                {
                    case "120x170":
                        query = query.Where(p => p.Price120x170.HasValue);
                        break;
                    case "150x220":
                        query = query.Where(p => p.Price150x220.HasValue);
                        break;
                    case "200x290":
                        query = query.Where(p => p.Price200x290.HasValue);
                        break;
                    default:
                        // Check for custom sizes
                        query = query.Where(p => p.CustomSizes != null && p.CustomSizes.Any(cs => cs.SizeName.ToLower() == size.ToLower() && cs.IsActive));
                        break;
                }
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

            var totalCount = query.Count();
            var results = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.PriceRange = priceRange;
            ViewBag.Category = category;
            ViewBag.Size = size;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.AllCategories = _db?.Categories?.OrderBy(c => c.Name).Select(c => c.Name).ToList() ?? new List<string>();
            ViewBag.AllSizes = GetAllAvailableSizes();
            ViewBag.ResultsCount = results.Count;
            return View(results);
        }

        // Helper method to get all available sizes
        private List<string> GetAllAvailableSizes()
        {
            var sizes = new List<string>();
            
            // Add standard sizes
            sizes.Add("120x170");
            sizes.Add("150x220");
            sizes.Add("200x290");
            
            // Add custom sizes
            List<string> customSizes = new List<string>();
            try
            {
                customSizes = _db?.ProductSizes?
                    .Where(ps => ps.IsActive)
                    .Select(ps => ps.SizeName)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList() ?? new List<string>();
            }
            catch
            {
                // Table doesn't exist yet, return empty list
                customSizes = new List<string>();
            }
            
            sizes.AddRange(customSizes);
            
            return sizes;
        }

        // Fuzzy search helper method
        private bool FuzzyMatch(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
                return false;

            // Simple fuzzy matching: check if search term characters appear in order
            int searchIndex = 0;
            foreach (char c in text)
            {
                if (searchIndex < searchTerm.Length && char.ToLower(c) == char.ToLower(searchTerm[searchIndex]))
                {
                    searchIndex++;
                }
            }
            
            // Consider it a match if we found at least 70% of the search term characters
            return searchIndex >= (searchTerm.Length * 0.7);
        }

        // Search suggestions endpoint
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetSearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<string>());
            }

            var suggestions = new List<string>();
            var products = productRepository.GetAllProducts().ToList();
            var loweredTerm = term.ToLower();

            // Get product names that contain the search term
            var productNames = products
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(loweredTerm))
                .Select(p => p.Name!)
                .Distinct()
                .Take(5)
                .ToList();

            // Get categories that contain the search term (from both products and categories table)
            var productCategories = products
                .Where(p => !string.IsNullOrEmpty(p.Category) && p.Category.ToLower().Contains(loweredTerm))
                .Select(p => p.Category!)
                .Distinct()
                .ToList();

            var dbCategories = _db?.Categories?
                .Where(c => c.Name.ToLower().Contains(loweredTerm))
                .Select(c => c.Name)
                .Distinct()
                .ToList() ?? new List<string>();

            var allCategories = productCategories.Union(dbCategories).Distinct().Take(3).ToList();

            // Get custom sizes that contain the search term
            List<string> customSizes = new List<string>();
            try
            {
                customSizes = _db?.ProductSizes?
                    .Where(ps => ps.IsActive && ps.SizeName.ToLower().Contains(loweredTerm))
                    .Select(ps => ps.SizeName)
                    .Distinct()
                    .Take(3)
                    .ToList() ?? new List<string>();
            }
            catch
            {
                // Table doesn't exist yet, return empty list
                customSizes = new List<string>();
            }

            suggestions.AddRange(productNames);
            suggestions.AddRange(allCategories);
            suggestions.AddRange(customSizes);

            return Json(suggestions.Take(8).ToList());
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

        // Custom Size Management
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCustomSize(int productId, string sizeName, decimal price, string? description = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sizeName) || price <= 0)
                {
                    TempData["Error"] = "Size name and price are required.";
                    return RedirectToAction("Edit", new { id = productId });
                }

                var product = _db.Products.Find(productId);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                // Check if size already exists for this product
                var existingSize = _db.ProductSizes.FirstOrDefault(ps => ps.ProductId == productId && ps.SizeName == sizeName);
                if (existingSize != null)
                {
                    TempData["Error"] = $"Size '{sizeName}' already exists for this product.";
                    return RedirectToAction("Edit", new { id = productId });
                }

                var customSize = new ProductSize
                {
                    ProductId = productId,
                    SizeName = sizeName,
                    Price = price,
                    Description = description,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _db.ProductSizes.Add(customSize);
                _db.SaveChanges();

                TempData["Success"] = $"Custom size '{sizeName}' added successfully.";
                return RedirectToAction("Edit", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Custom sizes feature is not available yet. Please create the ProductSizes table first.";
                return RedirectToAction("Edit", new { id = productId });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateCustomSize(int sizeId, string sizeName, decimal price, string? description = null)
        {
            var customSize = _db.ProductSizes.Find(sizeId);
            if (customSize == null)
            {
                TempData["Error"] = "Size not found.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(sizeName) || price <= 0)
            {
                TempData["Error"] = "Size name and price are required.";
                return RedirectToAction("Edit", new { id = customSize.ProductId });
            }

            customSize.SizeName = sizeName;
            customSize.Price = price;
            customSize.Description = description;

            _db.SaveChanges();

            TempData["Success"] = $"Custom size '{sizeName}' updated successfully.";
            return RedirectToAction("Edit", new { id = customSize.ProductId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCustomSize(int sizeId)
        {
            var customSize = _db.ProductSizes.Find(sizeId);
            if (customSize == null)
            {
                TempData["Error"] = "Size not found.";
                return RedirectToAction("Index");
            }

            var productId = customSize.ProductId;
            _db.ProductSizes.Remove(customSize);
            _db.SaveChanges();

            TempData["Success"] = $"Custom size '{customSize.SizeName}' deleted successfully.";
            return RedirectToAction("Edit", new { id = productId });
        }
    }
}

