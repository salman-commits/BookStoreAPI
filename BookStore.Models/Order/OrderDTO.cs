using BookStore.Models.Account;
using System;

namespace BookStore.Models.Order
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserEmail { get; set; }
        public Address Address { get; set; }
        public int DeliveryMethodId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
