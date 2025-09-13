using CarpetStore.Models;

namespace CarpetStore.Models.Interfaces
{
    public interface IProductStockRepository
    {
        List<ProductStock> GetProductStocks(int productId);
        ProductStock? GetProductStock(int productId, string dimension);
        void UpdateStock(int productId, string dimension, int quantity);
        void ReserveStock(int productId, string dimension, int quantity);
        void ReleaseStock(int productId, string dimension, int quantity);
        void ConsumeStock(int productId, string dimension, int quantity);
        bool IsInStock(int productId, string dimension, int quantity = 1);
        List<ProductStock> GetAllStocks();
        void CreateStock(int productId, string dimension, int quantity);
    }
}








