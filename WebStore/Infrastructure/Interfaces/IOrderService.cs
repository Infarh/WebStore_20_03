using System.Collections.Generic;
using WebStore.Domain.Entities.Orders;
using WebStore.ViewModels;
using WebStore.ViewModels.Orders;

namespace WebStore.Infrastructure.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetUserOrders(string UserName);

        Order GetOrderById(int id);

        Order CreateOrder(string UserName, CartViewModel Cart, OrderViewModel OrderModel);
    }
}
