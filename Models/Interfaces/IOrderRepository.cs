using CarpetStore.Models;

namespace CarpetStore.Models.Interfaces
{
    public interface IOrderRepository
    {
        void PlaceOrder(Order order);
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        void UpdateOrder(Order order);
    }
}
