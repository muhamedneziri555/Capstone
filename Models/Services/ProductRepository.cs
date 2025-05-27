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
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }


        public Product GetProductById(int id)
        {
            return dbContext.Products.Find(id);
        }

        public void UpdateProduct(Product product)
        {
            dbContext.Products.Update(product);
            dbContext.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = dbContext.Products.Find(id);
            if (product != null)
            {
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
            }
        }
    }
}


