using CarpetStore.Models;

namespace CarpetStore.Models.Interfaces
{
    public interface IOrderRepository
    {
        void PlaceOrder(Order order);
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        IEnumerable<Order> GetOrdersByUserId(string userId);
        void UpdateOrder(Order order);
    }
}
