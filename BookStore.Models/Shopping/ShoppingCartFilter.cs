using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Shopping
{
    public class ShoppingCartFilter
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public bool DeleteBook { get; set; }
        public int OrderId { get; set; }
    }
}
