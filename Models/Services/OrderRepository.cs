using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using CarpetStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CarpetStoreWebDb _dbContext;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public OrderRepository(CarpetStoreWebDb dbContext, IShoppingCartRepository shoppingCartRepository)
        {
            _dbContext = dbContext;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public void PlaceOrder(Order order)
        {
            try
            {
                var shoppingCartItems = _shoppingCartRepository.GetShoppingCartItems();
                if (!shoppingCartItems.Any())
                {
                    throw new Exception("Shopping cart is empty");
                }

                order.OrderDetails = new List<OrderDetail>();
                decimal total = 0;

                foreach (var item in shoppingCartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        Quantity = item.Qty,
                        ProductId = item.Product.Id,
                        Price = item.Product.Price,
                        Product = item.Product
                    };
                    order.OrderDetails.Add(orderDetail);
                    total += item.Product.Price * item.Qty;
                }

                order.OrderTotal = total;
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error placing order: {ex.Message}", ex);
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate);
        }

        public Order GetOrderById(int id)
        {
            return _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            return _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate);
        }

        public void UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
        }
    }
}

