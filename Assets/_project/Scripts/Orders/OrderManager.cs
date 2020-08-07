using System.Collections.Generic;

namespace FeedTheBaby
{
    public class OrderManager
    {
        List<Order> orders = new List<Order>();

        public OrderManager()
        {
        }


        public void AddOrder(Order order)
        {
            orders.Add(order);
        }
    }
}