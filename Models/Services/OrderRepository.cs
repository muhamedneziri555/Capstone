using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private CarpetStoreWebDb dbContext;
        private IShoppingCartRepository shoppingCartRepository;
        public OrderRepository(CarpetStoreWebDb dbContext, IShoppingCartRepository shoppingCartRepository)
        {
            this.dbContext = dbContext;
            this.shoppingCartRepository = shoppingCartRepository;
        }

        public void PlaceOrder(Order order)
        {
            var shoppingCartItems = shoppingCartRepository.GetShoppingCartItems();
            order.OrderDetails = new List<OrderDetail>();

            foreach (var item in shoppingCartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Qty,
                    ProductId = item.Product.Id,
                    Price = item.Product.Price
                };
                order.OrderDetails.Add(orderDetail);
            }

            order.OrderPlaced = DateTime.Now;
            order.OrderTotal = shoppingCartRepository.GetShoppingCartTotal();
            order.OrderStatus = "Pending"; // Set initial status

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderPlaced);
        }

        public Order? GetOrderById(int id)
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public void UpdateOrder(Order order)
        {
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
        }
    }
}

