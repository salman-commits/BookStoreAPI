using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Shopping
{
    public class ShoppingCartViewModel
    {        
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public decimal ShoppingCartTotal { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal ShoppingCartGrandTotal { get; set; }
    }
}
