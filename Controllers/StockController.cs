using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StockController : Controller
    {
        private readonly IProductStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        public StockController(IProductStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var stocks = _stockRepository.GetAllStocks();
            return View(stocks);
        }

        public IActionResult Manage(int productId)
        {
            var product = _productRepository.GetProductDetail(productId);
            if (product == null)
            {
                return NotFound();
            }

            var stocks = _stockRepository.GetProductStocks(productId);
            ViewBag.Product = product;
            ViewBag.Dimensions = new[] { "120x170", "150x220", "200x290" };
            
            return View(stocks);
        }

        [HttpPost]
        public IActionResult UpdateStock(int productId, string dimension, int quantity)
        {
            try
            {
                _stockRepository.UpdateStock(productId, dimension, quantity);
                TempData["Success"] = $"Stock updated successfully for {dimension}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating stock: {ex.Message}";
            }

            return RedirectToAction("Manage", new { productId });
        }

        [HttpPost]
        public IActionResult CreateStock(int productId, string dimension, int quantity)
        {
            try
            {
                _stockRepository.CreateStock(productId, dimension, quantity);
                TempData["Success"] = $"Stock created successfully for {dimension}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating stock: {ex.Message}";
            }

            return RedirectToAction("Manage", new { productId });
        }

        [HttpPost]
        public IActionResult BulkUpdate(int productId, int stock120x170, int stock150x220, int stock200x290)
        {
            try
            {
                _stockRepository.UpdateStock(productId, "120x170", stock120x170);
                _stockRepository.UpdateStock(productId, "150x220", stock150x220);
                _stockRepository.UpdateStock(productId, "200x290", stock200x290);
                
                TempData["Success"] = "All stock levels updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating stock: {ex.Message}";
            }

            return RedirectToAction("Manage", new { productId });
        }

        [HttpGet]
        public IActionResult CheckStock(int productId, string dimension)
        {
            var stock = _stockRepository.GetProductStock(productId, dimension);
            if (stock != null)
            {
                return Json(new { 
                    isInStock = stock.IsInStock, 
                    availableStock = stock.AvailableStock,
                    totalStock = stock.StockQuantity,
                    reservedStock = stock.ReservedQuantity
                });
            }
            else
            {
                return Json(new { 
                    isInStock = false, 
                    availableStock = 0,
                    totalStock = 0,
                    reservedStock = 0
                });
            }
        }
    }
}
