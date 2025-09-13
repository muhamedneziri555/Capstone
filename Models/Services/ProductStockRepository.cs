using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
    public class ProductStockRepository : IProductStockRepository
    {
        private readonly CarpetStoreWebDb _dbContext;

        public ProductStockRepository(CarpetStoreWebDb dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProductStock> GetProductStocks(int productId)
        {
            return _dbContext.ProductStocks
                .Where(ps => ps.ProductId == productId)
                .OrderBy(ps => ps.Dimension)
                .ToList();
        }

        public ProductStock? GetProductStock(int productId, string dimension)
        {
            return _dbContext.ProductStocks
                .FirstOrDefault(ps => ps.ProductId == productId && ps.Dimension == dimension);
        }

        public void UpdateStock(int productId, string dimension, int quantity)
        {
            var stock = GetProductStock(productId, dimension);
            if (stock != null)
            {
                stock.StockQuantity = quantity;
            }
            else
            {
                CreateStock(productId, dimension, quantity);
            }
            _dbContext.SaveChanges();
        }

        public void ReserveStock(int productId, string dimension, int quantity)
        {
            var stock = GetProductStock(productId, dimension);
            if (stock != null && stock.AvailableStock >= quantity)
            {
                stock.ReservedQuantity += quantity;
                _dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException($"Insufficient stock for product {productId} dimension {dimension}");
            }
        }

        public void ReleaseStock(int productId, string dimension, int quantity)
        {
            var stock = GetProductStock(productId, dimension);
            if (stock != null)
            {
                stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - quantity);
                _dbContext.SaveChanges();
            }
        }

        public void ConsumeStock(int productId, string dimension, int quantity)
        {
            var stock = GetProductStock(productId, dimension);
            if (stock != null)
            {
                stock.StockQuantity = Math.Max(0, stock.StockQuantity - quantity);
                stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - quantity);
                _dbContext.SaveChanges();
            }
        }

        public bool IsInStock(int productId, string dimension, int quantity = 1)
        {
            var stock = GetProductStock(productId, dimension);
            return stock != null && stock.AvailableStock >= quantity;
        }

        public List<ProductStock> GetAllStocks()
        {
            return _dbContext.ProductStocks
                .Include(ps => ps.Product)
                .OrderBy(ps => ps.Product.Name)
                .ThenBy(ps => ps.Dimension)
                .ToList();
        }

        public void CreateStock(int productId, string dimension, int quantity)
        {
            var stock = new ProductStock
            {
                ProductId = productId,
                Dimension = dimension,
                StockQuantity = quantity,
                ReservedQuantity = 0,
            };
            _dbContext.ProductStocks.Add(stock);
            _dbContext.SaveChanges();
        }
    }
}

