using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.Entities.Orders;
using WebStore.Infrastructure.Interfaces;
using WebStore.ViewModels;
using WebStore.ViewModels.Orders;

namespace WebStore.Infrastructure.Services.InSQL
{
    public class SqlOrderService : IOrderService
    {
        private readonly WebStoreDB _db;
        private readonly UserManager<User> _UserManager;
        private readonly IProductData _ProductData;

        public SqlOrderService(WebStoreDB db, UserManager<User> UserManager, IProductData ProductData)
        {
            _db = db;
            _UserManager = UserManager;
            _ProductData = ProductData;
        }

        public IEnumerable<Order> GetUserOrders(string UserName) => _db.Orders
           .Include(order => order.User)
           .Include(order => order.OrderItems)
           .Where(order => order.User.UserName == UserName)
           .AsEnumerable();

        public Order GetOrderById(int id) => _db.Orders
           .Include(order => order.OrderItems)
           .FirstOrDefault(order => order.Id == id);

        public async Task<Order> CreateOrderAsync(string UserName, CartViewModel Cart, OrderViewModel OrderModel)
        {
            var user = await _UserManager.FindByNameAsync(UserName);
            if (user is null)
                throw new InvalidOperationException("Невозможно сформировать заказ - пользователь не определён");

            var order_products = _ProductData.GetProducts(new ProductFilter { Ids = Cart.Items.Select(p => p.Product.Id) });

            var order = new Order
            {
                Name = OrderModel.Name,
                Address = OrderModel.Address,
                Phone = OrderModel.Phone,
                User = user,
                Date = DateTime.Now,
                OrderItems = order_products.Join(
                    Cart.Items,
                    product => product.Id,
                    cart_item => cart_item.Product.Id,
                    (product, item) => new OrderItem
                    {
                        Price = product.Price,
                        Quantity = item.Quantity,
                        Product = product
                    }).ToArray()
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;
        }
    }
}
