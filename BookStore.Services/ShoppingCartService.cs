using BookStore.Models.Shopping;
using BookStore.Repository.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services
{
    public class ShoppingCartService
    {
        GenericRepository<ShoppingCartItem> shopRepo = new GenericRepository<ShoppingCartItem>();
        public static List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public static int UserId { get; set; }
        public static decimal ShoppingCartTotal { get; set; }
        public static decimal ShippingCharge
        {
            get
            {
                OrderService orderService = new OrderService();
                return orderService.GetShippingChargeForPaymentPendingByUser(UserId);
            }
        }
        public static decimal ShoppingCartGrandTotal
        {
            get { return (ShoppingCartTotal + ShippingCharge); }
        }
        public static int ShoppingCartTotalCount
        {
            get
            {
                return ShoppingCartItems == null ? 0 : ShoppingCartItems.Select(x => x.Quantity).Sum();
            }
        }
        public ShoppingCartService()
        {
        }
        public ShoppingCartService(int userId)
        {
            UserId = userId;
            GetShoppingCartItems();
        }
        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            ShoppingCartItems = shopRepo.Gets(c => c.UserId == UserId, null, "Book,Book.BookCategory").ToList();

            foreach (var book in ShoppingCartItems)
            {
                book.Book.ImageURL = @"Images\" + book.Book.ImageURL;
                book.Book.ImageThumbnailURL = @"Images\" + book.Book.ImageThumbnailURL;
            }
            return (ShoppingCartItems = ShoppingCartItems.OrderBy(b => b.Book.Title).ToList());
        }

        public int GetShoppingCartItemsByBook(int bookId)
        {
            int bookCount = 0;
            var book = shopRepo.Get(c => c.UserId == UserId && c.BookId == bookId);
            if (book != null)
                bookCount = book.Quantity;

            return bookCount;
        }

        public decimal GetShoppingCartTotal()
        {
            ShoppingCartTotal = ShoppingCartItems
                .Select(c => c.Book.Price * c.Quantity).Sum();
            return ShoppingCartTotal;
        }
        public int AddToCart(int bookId)
        {
            var shoppingCartItem =
                    shopRepo.Gets(
                        s => s.Book.Id == bookId && s.UserId == UserId).SingleOrDefault();

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    UserId = UserId,
                    BookId = bookId,
                    Quantity = 1
                };

                shopRepo.Insert(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity++;
            }
            shopRepo.Save();

            return shoppingCartItem.Quantity;
        }
        public int RemoveFromCart(int bookId, bool deleteBook)
        {
            var shoppingCartItem =
                    shopRepo.Gets(
                        s => s.Book.Id == bookId && s.UserId == UserId).SingleOrDefault();

            if (shoppingCartItem != null && deleteBook)
            {
                shopRepo.Delete(shoppingCartItem.ShoppingCartItemId);
                return 0;
            }
            else if (shoppingCartItem != null && shoppingCartItem.Quantity == 1)
            {
                shopRepo.Delete(shoppingCartItem.ShoppingCartItemId);
                return 0;
            }
            else
                shoppingCartItem.Quantity--;

            shopRepo.Save();

            return shoppingCartItem.Quantity;
        }
        public void DeleteBookFomCart(int bookId)
        {
            List<ShoppingCartItem> cartItems = shopRepo.Gets()
                .Where(c => c.UserId == UserId && c.BookId == bookId).ToList();

            shopRepo.Delete(cartItems);

            shopRepo.Save();
        }
        public bool DeleteShoppingCartPostPayment()
        {
            List<ShoppingCartItem> cartItems = shopRepo.Gets()
                .Where(c => c.UserId == UserId).ToList();

            if (cartItems.Count > 0)
            {
                shopRepo.Delete(cartItems);

                shopRepo.Save();
                return true;
            }
            return false;
        }
    }
}
