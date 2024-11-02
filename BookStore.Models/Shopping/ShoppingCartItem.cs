using System;

namespace BookStore.Models.Shopping
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public Book.Book Book { get; set; }
        public int BookId { get; set; }
    }
}
