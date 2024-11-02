namespace BookStore.Models.Order
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public Book.Book Book { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
