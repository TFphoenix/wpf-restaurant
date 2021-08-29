using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public static class ActiveSession
    {
        public static user ConnectedUser;
        public static List<ProductDisplay> Basket = new List<ProductDisplay>();

        public enum OrderState
        {
            Registered,
            Preparing,
            Delivering,
            Delivered,
            Canceled
        }
    }
}
