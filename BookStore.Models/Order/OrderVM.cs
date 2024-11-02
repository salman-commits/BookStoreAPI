using BookStore.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Order
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public decimal ShippingCharge { get; set; }        
        public decimal Subtotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
