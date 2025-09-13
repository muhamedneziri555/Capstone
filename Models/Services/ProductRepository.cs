using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
    public class ProductRepository : IProductRepository
    {
        private CarpetStoreWebDb dbContext;

        public ProductRepository(CarpetStoreWebDb dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return dbContext.Products
                .Include(p => p.CustomSizes)
                .AsNoTracking(); // Read-only optimization
        }

        public Product? GetProductDetail(int id)
        {
            return dbContext.Products
                .Include(p => p.CustomSizes)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbContext.Products
                .Where(p => p.IsTrendingProduct)
                .Include(p => p.CustomSizes)
                .AsNoTracking();
        }

        public void AddProduct(Product product)
        {
            // Sync base price to smallest size price by convention
            if (product.Price120x170.HasValue)
            {
                product.Price = product.Price120x170.Value;
            }

            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }

        public Product GetProductById(int id)
        {
            return dbContext.Products.Find(id)!;
        }

        public void UpdateProduct(Product product)
        {
            // Sync base price to smallest size price by convention
            if (product.Price120x170.HasValue)
            {
                product.Price = product.Price120x170.Value;
            }

            dbContext.Products.Update(product);
            dbContext.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            // First, remove all shopping cart items that reference this product
            var cartItems = dbContext.ShoppingCartItems.Where(s => s.Product != null && s.Product.Id == id).ToList();
            dbContext.ShoppingCartItems.RemoveRange(cartItems);

            // Then remove the product
            var product = dbContext.Products.Find(id);
            if (product != null)
            {
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
            }
        }
    }
}


