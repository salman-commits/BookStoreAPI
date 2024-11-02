using BookStore.Models.Account;
using System;
namespace BookStore.Models.Order
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public int DeliveryMethodId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
