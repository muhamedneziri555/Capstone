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
            return dbContext.Products;
        }

        public Product? GetProductDetail(int id)
        {
            return dbContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbContext.Products.Where(p => p.IsTrendingProduct);
        }

        public void AddProduct(Product product)
        {
            // Set category based on product name
            if (!string.IsNullOrEmpty(product.Name))
            {
                var firstWord = product.Name.Split(' ')[0];
                product.Category = firstWord + " Collection";
            }

            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }

        public Product GetProductById(int id)
        {
            return dbContext.Products.Find(id);
        }

        public void UpdateProduct(Product product)
        {
            // Update category based on product name
            if (!string.IsNullOrEmpty(product.Name))
            {
                var firstWord = product.Name.Split(' ')[0];
                product.Category = firstWord + " Collection";
            }

            dbContext.Products.Update(product);
            dbContext.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            // First, remove all shopping cart items that reference this product
            var cartItems = dbContext.ShoppingCartItems.Where(s => s.Product.Id == id).ToList();
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


