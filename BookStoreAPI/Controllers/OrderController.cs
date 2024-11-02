using BookStore.Models.Account;
using BookStore.Models.Order;
using BookStore.Repository.Repository;
using BookStore.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookStoreAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        GenericRepository<User> userRepo = new GenericRepository<User>();
        OrderService orderService = new OrderService();

        [Authorize]
        [Route("GetOrderedDetailsByUser")]
        [HttpGet]
        //api/Order/GetOrderedDetailsByUser/salman
        public HttpResponseMessage GetOrderedDetailsByUser(string userEmail)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userEmail);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var orderDetail = orderService.GetOrderedDetailsByUser(user.UserId);

                return Request.CreateResponse(HttpStatusCode.OK, orderDetail, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize]
        [Route("GetOrderDetailsByOrderId")]
        [HttpGet]
        //api/Order/GetOrderDetailsByOrderId/1
        public HttpResponseMessage GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                if (orderId == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Order id not valid", Configuration.Formatters.JsonFormatter);

                var orderDetailVM = orderService.GetOrderDetailsByOrderId(orderId);

                return Request.CreateResponse(HttpStatusCode.OK, orderDetailVM, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize]
        [Route("GetDeliveryMethod")]
        [HttpGet]
        //api/Order/GetDeliveryMethod
        public HttpResponseMessage GetDeliveryMethod()
        {
            try
            {
                var deliveryMethods = orderService.GetDeliveryMethod();
                return Request.CreateResponse(HttpStatusCode.OK, deliveryMethods, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize]
        [Route("AddUpdateOrderDetailsByUser")]
        [HttpPost]
        //api/Order/AddUpdateOrderDetailsByUser/salman
        public HttpResponseMessage AddUpdateOrderDetailsByUser(string userEmail, [FromBody] Order order)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userEmail);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                if (order != null)
                {
                    Address addressDB = new Address();
                    Address address = null;
                    order.UserId = user.UserId;

                    // create or update address
                    if (order.Address != null)
                    {
                        order.Address.UserId = user.UserId;
                        order.Address.IsPrimary = true;
                        address = orderService.AddUpdateAddress(order);
                        order.Address = address;
                        order.AddressId = address.AddressId;
                    }

                    // create or update order                
                    order.OrderStatus = (order.OrderStatus == null) ? OrderStatus.PaymentPending : order.OrderStatus;
                    order.DeliveryMethodId = (order.DeliveryMethodId == 0) ? 1 : order.DeliveryMethodId;
                    addressDB = order.Address;
                    order.Address = null;
                    order = orderService.AddUpdateOrder(order);
                    order.Address = addressDB;

                    // create or update OrderItem

                    List<OrderItem> orderItems = orderService.AddUpdateOrderItems(order);
                    //TODO: if orderItems count not greater than zero then return error
                }
                return Request.CreateResponse(HttpStatusCode.OK, order, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize]
        [Route("GetOrderDetailsByUser")]
        [HttpGet]
        //api/Order/GetOrderDetailsByUser/salman
        public HttpResponseMessage GetOrderDetailsByUser(string userEmail)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userEmail);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var orderDetail = orderService.GetOrderByUser(user.UserId);

                return Request.CreateResponse(HttpStatusCode.OK, orderDetail, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize]
        [Route("GetAddressByType")]
        [HttpGet]
        //api/Order/GetAddressByType
        public HttpResponseMessage GetAddressByType(string userEmailId, int addressTypeId)
        {
            try
            {
                User user = userRepo.Get(u => u.Email == userEmailId);
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");

                var address = orderService.GetAddressByType(addressTypeId, user.UserId);
                return Request.CreateResponse(HttpStatusCode.OK, address, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetAddressType")]
        [HttpGet]
        //api/Order/GetAddressType
        public HttpResponseMessage GetAddressType()
        {
            try
            {
                var addressType = orderService.GetAddressTypeList();
                return Request.CreateResponse(HttpStatusCode.OK, addressType, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}