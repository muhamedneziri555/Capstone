namespace CarpetStore.Models.Interfaces
{
    public interface IOrderRepository
    {
        void PlaceOrder(Order order);
    }
}
