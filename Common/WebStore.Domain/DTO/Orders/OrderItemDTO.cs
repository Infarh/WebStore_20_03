using WebStore.Domain.Entities.Base;

namespace WebStore.Domain.DTO.Orders
{
    /// <summary>Модель передачи данных по позиции заказа</summary>
    public class OrderItemDTO : BaseEntity
    {
        /// <summary>Цена</summary>
        public decimal Price { get; set; }

        /// <summary>Количество</summary>
        public int Quantity { get; set; }
    }
}