using System.Collections.Generic;
using System.Linq;

namespace WebStore.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<(ProductViewModel Product, int Quantity)> Items { get; set; } = new List<(ProductViewModel Product, int Quantity)>();

        public int ItemsCount => Items?.Sum(item => item.Quantity) ?? 0;

        public decimal TotalSum => Items?.Sum(item => item.Product.Price * item.Quantity) ?? 0m;
    }
}
