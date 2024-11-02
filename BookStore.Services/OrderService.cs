using BookStore.Models.Account;
using BookStore.Models.Order;
using BookStore.Models.Shopping;
using BookStore.Repository.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services
{
    public class OrderService
    {
        GenericRepository<Order> orderRepo = new GenericRepository<Order>();
        GenericRepository<OrderItem> orderItemRepo = new GenericRepository<OrderItem>();
        GenericRepository<Address> addressRepo = new GenericRepository<Address>();
        ShoppingCartService shoppingCartService = new ShoppingCartService();
        GenericRepository<DeliveryMethod> deliveryMethodRepo = new GenericRepository<DeliveryMethod>();
        GenericRepository<AddressType> addressTypeRepo = new GenericRepository<AddressType>();
        public decimal GetShippingChargeForPaymentPendingByUser(int userId)
        {
            ShoppingCartService.UserId = userId;
            var order = orderRepo.Get(c => c.UserId == userId && c.OrderStatus == OrderStatus.PaymentPending, null, "DeliveryMethod");

            if (order == null)
                return 0;

            return order.DeliveryMethod.Price;
        }
        public decimal GetShippingChargeForOrderedViewByUser(int userId, int orderId)
        {
            ShoppingCartService.UserId = userId;
            Order order = null;
            if (orderId > 0)
            {
                order = orderRepo.Get(c => c.OrderId == orderId && (c.OrderStatus == OrderStatus.PaymentCompleted || c.OrderStatus == OrderStatus.PaymentFailed), null, "DeliveryMethod");
            }
            else
            {
                order = orderRepo.Get(c => c.UserId == userId && (c.OrderStatus == OrderStatus.PaymentCompleted || c.OrderStatus == OrderStatus.PaymentFailed), null, "DeliveryMethod");
            }
            if (order == null)
                return 0;

            return order.DeliveryMethod.Price;
        }
        public Order GetOrderByUser(int userId)
        {
            var order = orderRepo.Get(c => c.UserId == userId && c.OrderStatus == OrderStatus.PaymentPending, null, "User,Address,Address.AddressType,DeliveryMethod");

            if (order == null)
                return null;

            return order;
        }
        public List<OrderItem> GetOrderItemById(int orderId)
        {
            var orderItems = orderItemRepo.Gets(c => c.OrderId == orderId, null, "Book, Book.BookCategory").ToList();

            if (orderItems == null) return null;

            return orderItems;
        }
        public List<DeliveryMethod> GetDeliveryMethod()
        {
            List<DeliveryMethod> deliveryMethods =
                deliveryMethodRepo.GetAll().OrderBy(n => n.Price).ToList();

            if (deliveryMethods == null) return null;

            return deliveryMethods;
        }
        public Address GetAddressByType(int addressTypeId, int userId)
        {
            var address = addressRepo.Get(x => x.AddressTypeId == addressTypeId && x.UserId == userId);
            return address;
        }
        public List<AddressType> GetAddressTypeList()
        {
            return addressTypeRepo.Gets().ToList();
        }
        public Address AddUpdateAddress(Order order)
        {
            Address address = addressRepo.Get(x => x.AddressTypeId == order.Address.AddressTypeId && x.UserId == order.UserId);

            if (address == null)
                addressRepo.Insert(order.Address);
            else
            {
                address.AddressLine1 = order.Address.AddressLine1;
                address.AddressLine2 = order.Address.AddressLine2;
                address.AddressTypeId = order.Address.AddressTypeId;
                address.City = order.Address.City;
                address.Country = order.Address.Country;
                address.IsPrimary = order.Address.IsPrimary;
                address.PhoneNumber = order.Address.PhoneNumber;
                address.State = order.Address.State;
                address.UserId = order.Address.UserId;
                address.ZipCode = order.Address.ZipCode;
                addressRepo.Update(address);

                return address;
            }
            addressRepo.Save();
            return order.Address;
        }
        public Order AddUpdateOrder1(Order order)
        {
            int addressId = order.AddressId;
            Order orderDB = orderRepo.Get(x => x.OrderId == order.OrderId && x.Address.AddressTypeId == order.Address.AddressTypeId, null, "Address");

            if (orderDB == null)
                orderRepo.Insert(order);
            else
            {
                orderDB.Address.AddressLine1 = order.Address.AddressLine1;
                orderDB.Address.AddressLine2 = order.Address.AddressLine2;
                orderDB.Address.AddressTypeId = order.Address.AddressTypeId;
                orderDB.Address.IsPrimary = order.Address.IsPrimary;
                orderDB.Address.City = order.Address.City;
                orderDB.Address.State = order.Address.State;
                orderDB.Address.Country = order.Address.Country;
                orderDB.Address.PhoneNumber = order.Address.PhoneNumber;
                orderDB.Address.ZipCode = order.Address.ZipCode;
                orderDB.Address.UserId = order.Address.UserId;

                orderDB.AddressId = addressId;
                orderRepo.Update(orderDB);
            }
            orderRepo.Save();
            return order;
        }
        public Order AddUpdateOrder(Order order)
        {
            int addressId = order.AddressId;
            if (order.OrderId == 0)
                orderRepo.Insert(order);
            else
            {
                order.AddressId = addressId;
                orderRepo.Update(order);
            }

            orderRepo.Save();
            return order;
        }
        public Order UpdateOrderStatus(int orderId)
        {
            var order = orderRepo.Get(c => c.OrderId == orderId);

            if (order == null)
                return null;
            else
            {
                order.OrderStatus = OrderStatus.PaymentCompleted;
                orderRepo.Update(order);
                orderRepo.Save();
            }
            return order;
        }

        public List<OrderItem> AddUpdateOrderItems(Order order)
        {
            List<OrderItem> orderItems = GetOrderItemById(order.OrderId);
            List<OrderItem> orderItemsDB = new List<OrderItem>();

            if (orderItems != null && orderItems.Count > 0)
            {
                orderItemRepo.Delete(orderItems);
            }

            ShoppingCartService.UserId = order.UserId;
            List<ShoppingCartItem> shoppingCartItems = shoppingCartService.GetShoppingCartItems();
            if (shoppingCartItems != null && shoppingCartItems.Count > 0)
            {
                foreach (var item in shoppingCartItems)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.OrderId = order.OrderId;
                    orderItem.BookId = item.BookId;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Price = item.Book.Price;

                    orderItemsDB.Add(orderItem);
                }

                orderItemRepo.Insert(orderItemsDB);

                orderItemRepo.Save();
            }

            return orderItemsDB;
        }
        public bool UpdateOrderItemsOnBookAction(int userId)
        {
            var order = orderRepo.Get(c => c.UserId == userId && c.OrderStatus == OrderStatus.PaymentPending, null, "User,Address,Address.AddressType,DeliveryMethod");

            if (order == null)
                return false;

            var oderItems = AddUpdateOrderItems(order);
            if (oderItems.Count == 0) //remove order entry which is pending
            {
                DeleteOrderOnUserRemovingAllBooksBeforeFinalOrder(order);
            }

            return (oderItems.Count > 0);
        }
        public Order DeleteOrderOnUserRemovingAllBooksBeforeFinalOrder(Order order)
        {
            if (order.OrderId > 0)
                orderRepo.Delete(order);

            orderRepo.Save();

            return order;
        }
        public List<OrderMasterVM> GetOrderedDetailsByUser(int userId)
        {
            var orders = orderRepo.Gets(c => c.UserId == userId && c.OrderStatus == OrderStatus.PaymentCompleted, null, "User,Address,Address.AddressType,DeliveryMethod");

            if (orders == null) return null;

            List<OrderMasterVM> orderVM = new List<OrderMasterVM>();
            foreach (var order in orders)
            {
                OrderMasterVM orderRow = new OrderMasterVM();
                orderRow.OrderId = order.OrderId;
                orderRow.OrderStatus = order.OrderStatus.ToString();
                orderRow.OrderDate = order.OrderDate;
                orderRow.OrderGrandTotal = GetOrderGrandTotalByOrderId(order.OrderId, userId);
                orderVM.Add(orderRow);
            }

            return orderVM;
        }
        private decimal GetOrderGrandTotalByOrderId(int orderId, int userId)
        {
            List<OrderItem> orderItems = GetOrderItemById(orderId);
            if (orderItems == null) return 0;
            decimal shippingCharge = GetShippingChargeForOrderedViewByUser(userId, orderId);
            return ((orderItems.Select(c => c.Book.Price * c.Quantity).Sum()) + shippingCharge);
        }
        public OrderVM GetOrderDetailsByOrderId(int orderId)
        {
            var order = orderRepo.Get(c => c.OrderId == orderId, null, "DeliveryMethod");

            if (order == null) return null;

            decimal shippingCharge = GetShippingChargeForOrderedViewByUser(0, order.OrderId);
            List<OrderItem> orderItems = GetOrderItemById(order.OrderId);

            OrderVM orderViewModel = new OrderVM
            {
                OrderId = orderId,
                ShippingCharge = shippingCharge,
                OrderItems = orderItems,
                Subtotal = ((orderItems.Select(c => c.Book.Price * c.Quantity).Sum())),
                GrandTotal = ((orderItems.Select(c => c.Book.Price * c.Quantity).Sum()) + shippingCharge)
            };

            foreach (var book in orderViewModel.OrderItems)
            {
                book.Book.ImageURL = @"Images\" + book.Book.ImageURL;
                book.Book.ImageThumbnailURL = @"Images\" + book.Book.ImageThumbnailURL;
            }
            return orderViewModel;
        }
    }
}
