using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Models.Services
{
    class OrderServices
    {
        private readonly RestaurantEDM _context = new RestaurantEDM();

        private void CanPlaceOrder()
        {
            foreach (var productDisplay in ActiveSession.Basket)
            {
                var maxQuantity = _context.spGetProductAvailability(productDisplay.Id).ToList()[0];
                if (productDisplay.Quantity > maxQuantity)
                    throw new ArgumentOutOfRangeException($"Can't order {productDisplay.Quantity} x {productDisplay.Name}\nMaximum quantity available for this product is: {maxQuantity}");
            }
        }

        public void PlaceOrder(double orderDiscount, double orderDeliveryCost, double orderTotalPrice)
        {
            // Verify order products availability
            CanPlaceOrder();

            // Create order
            order orderToPlace = new order
            {
                user_id = ActiveSession.ConnectedUser.id,
                state = (int)ActiveSession.OrderState.Registered,
                timestamp = DateTime.Now,
                estimative_delivery_time = DateTime.Now + TimeSpan.FromMinutes(ActiveSession.Basket.Count * 10 + orderTotalPrice / 10),
                discount = orderDiscount,
                delivery_cost = orderDeliveryCost,
                total_price = orderTotalPrice,
                order_product = new List<order_product>()
            };
            _context.orders.Add(orderToPlace);

            foreach (var productDisplay in ActiveSession.Basket)
            {
                // Add product on order
                orderToPlace.order_product.Add(new order_product
                {
                    product_id = productDisplay.Id,
                    quantity = productDisplay.Quantity
                });

                // Adjust quantities
                var productQuery = from contextProduct in _context.products
                                   where contextProduct.id.Equals(productDisplay.Id)
                                   select contextProduct;
                var productItem = productQuery.ToList()[0];
                if (productItem.is_menu)
                {
                    // Menu
                    var menuQuery = from contextMenu in _context.menus
                                    where contextMenu.product_id.Equals(productItem.id)
                                    select contextMenu;
                    var menuItem = menuQuery.ToList()[0];

                    foreach (var menuDish in menuItem.menu_dish)
                        menuDish.dish.total_quantity -= menuDish.quantity * productDisplay.Quantity;
                }
                else
                {
                    // Dish
                    var dishQuery = from contextDish in _context.dishes
                                    where contextDish.product_id.Equals(productItem.id)
                                    select contextDish;
                    var dishItem = dishQuery.ToList()[0];

                    dishItem.total_quantity -= dishItem.portion_quantity * productDisplay.Quantity;
                }
            }

            _context.SaveChanges();
        }

        public List<OrderDisplay> GetAllOrderDisplays()
        {
            List<OrderDisplay> orderDisplays = new List<OrderDisplay>();
            var query = from contextOrder in _context.orders
                        orderby contextOrder.timestamp descending
                        select contextOrder;

            foreach (var orderItem in query.ToList())
                orderDisplays.Add(new OrderDisplay(orderItem));

            return orderDisplays;
        }

        public List<OrderDisplay> GetUserOrderDisplays(int userId)
        {
            List<OrderDisplay> orderDisplays = new List<OrderDisplay>();
            var query = from contextOrder in _context.orders
                        where contextOrder.user_id.Equals(userId)
                        select contextOrder;

            foreach (var orderItem in query.ToList())
                orderDisplays.Add(new OrderDisplay(orderItem));

            return orderDisplays;
        }

        public order GetUserLastOrder(int userId)
        {
            var query = from contextOrder in _context.orders
                        where contextOrder.user_id.Equals(userId)
                        orderby contextOrder.timestamp descending
                        select contextOrder;
            return query.ToList()[0];
        }

        public string GetOrderProductsAsString(order orderItem)
        {
            string orderProductsString = "Order consists of:\n";

            foreach (var orderProduct in orderItem.order_product)
            {
                orderProductsString += $"{orderProduct.quantity} x {orderProduct.product.name}\n";
            }

            return orderProductsString;
        }

        public string GetOrderCustomerInfoAsString(order orderItem)
        {
            return $"Order placed by:\n{orderItem.user.first_name} {orderItem.user.last_name}\nEmail: {orderItem.user.email}\nAddress: {orderItem.user.address}\nPhone: {orderItem.user.phone}";
        }

        public void SetOrderState(int orderId, int orderState)
        {
            var orderItem = (from contextOrder in _context.orders
                             where contextOrder.id.Equals(orderId)
                             select contextOrder).First();

            orderItem.state = orderState;
            _context.SaveChanges();
        }
    }
}
