using BookStore.Models.Account;
using BookStore.Models.Book;
using BookStore.Models.Order;
using BookStore.Models.Shopping;
using BookStore.Repository.Repository;
using BookStore.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookStoreAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ShoppingCart")]
    public class ShoppingCartController : ApiController
    {
        GenericRepository<Book> bookRepo = new GenericRepository<Book>();
        GenericRepository<User> userRepo = new GenericRepository<User>();
        readonly OrderService orderService = new OrderService();
        ShoppingCartService shoppingCartServices = new ShoppingCartService();

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetShoppingCartDetailsByUser")]
        [HttpGet]
        //api/ShoppingCart/GetShoppingCartDetailsByUser/salman
        public HttpResponseMessage GetShoppingCartDetailsByUser(string userId)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                ShoppingCartService.UserId = user.UserId;
                shoppingCartServices.GetShoppingCartItems();
                var shoppingCartViewModel = new ShoppingCartViewModel
                {
                    ShoppingCartItems = ShoppingCartService.ShoppingCartItems,
                    ShoppingCartTotal = shoppingCartServices.GetShoppingCartTotal(),
                    ShippingCharge = ShoppingCartService.ShippingCharge,
                    ShoppingCartGrandTotal = ShoppingCartService.ShoppingCartGrandTotal
                };

                return Request.CreateResponse(HttpStatusCode.OK, shoppingCartViewModel, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetShoppingCartTotalCountByUser")]
        [HttpGet]
        public HttpResponseMessage GetShoppingCartTotalCountByUser(string userId)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");


                shoppingCartServices = new ShoppingCartService(user.UserId);
                int shoppingCartCount = ShoppingCartService.ShoppingCartTotalCount;

                return Request.CreateResponse(HttpStatusCode.OK, shoppingCartCount, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetShoppingCartQuantityByBook")]
        [HttpGet]
        public HttpResponseMessage GetShoppingCartQuantityByBook(int bookId, string userId)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                ShoppingCartService.UserId = user.UserId;
                int shoppingCartBookCount = shoppingCartServices.GetShoppingCartItemsByBook(bookId);

                return Request.CreateResponse(HttpStatusCode.OK, shoppingCartBookCount, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("AddToShoppingCart")]
        [HttpPost]
        public HttpResponseMessage AddToShoppingCart([FromBody] ShoppingCartFilter cart)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == cart.UserId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var selectedBook = bookRepo.Gets(b => b.Id == cart.BookId).FirstOrDefault();
                if (selectedBook == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Book Not Found");

                ShoppingCartService.UserId = user.UserId;
                int bookCount = shoppingCartServices.AddToCart(selectedBook.Id);

                bool orderItemsUpdated = false;
                if (bookCount > 0)
                {
                    orderItemsUpdated = orderService.UpdateOrderItemsOnBookAction(user.UserId);
                }

                return Request.CreateResponse(HttpStatusCode.Created, bookCount, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("RemoveFromShoppingCart")]
        [HttpPost]
        public HttpResponseMessage RemoveFromShoppingCart([FromBody] ShoppingCartFilter cart)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == cart.UserId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var selectedBook = bookRepo.Gets(b => b.Id == cart.BookId).FirstOrDefault();
                if (selectedBook == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Book Not Found");

                ShoppingCartService.UserId = user.UserId;
                int bookCount = shoppingCartServices.RemoveFromCart(selectedBook.Id, cart.DeleteBook);

                bool orderItemsUpdated = false;
                orderItemsUpdated = orderService.UpdateOrderItemsOnBookAction(user.UserId);


                return Request.CreateResponse(HttpStatusCode.Accepted, bookCount, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [Route("DeleteBookFromCart")]
        [HttpDelete]
        //api/ShoppingCart/DeleteBookFromCart
        public HttpResponseMessage DeleteBookFromCart([FromBody] ShoppingCartFilter cart)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == cart.UserId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var selectedBook = bookRepo.Gets(b => b.Id == cart.BookId).FirstOrDefault();
                if (selectedBook == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Book Not Found");

                ShoppingCartService.UserId = user.UserId;
                shoppingCartServices.DeleteBookFomCart(selectedBook.Id);

                return Request.CreateResponse(HttpStatusCode.Accepted, "Book Removed from Shopping Cart", Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [Route("DeleteShoppingCartPostPayment")]
        [HttpPost]
        //api/ShoppingCart/DeleteShoppingCart
        public HttpResponseMessage DeleteShoppingCartPostPayment([FromBody] ShoppingCartFilter cart)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == cart.UserId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                ShoppingCartService.UserId = user.UserId;
                bool isDeleted = shoppingCartServices.DeleteShoppingCartPostPayment();
                string result = string.Empty;
                Order order = null;

                if (isDeleted)
                {
                    order = orderService.UpdateOrderStatus(cart.OrderId);
                }

                return Request.CreateResponse(HttpStatusCode.Accepted, order, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}