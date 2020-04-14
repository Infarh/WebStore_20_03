using System;
using System.Collections.Generic;
using WebStore.Domain.Entities.Base;

namespace WebStore.Domain.DTO.Orders
{
    /// <summary>Модель передачи данных по заказу</summary>
    public class OrderDTO : NamedEntity
    {
        /// <summary>Телефон</summary>
        public string Phone { get; set; }

        /// <summary>Адрес доставки</summary>
        public string Address { get; set; }

        /// <summary>Дата формирования</summary>
        public DateTime Date { get; set; }

        /// <summary>Перечень позиций</summary>
        public IEnumerable<OrderItemDTO> OrderItems { get; set; }
    }
}
