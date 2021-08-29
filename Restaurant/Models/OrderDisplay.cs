using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.ViewModels;

namespace Restaurant.Models
{
    class OrderDisplay : ViewModel
    {
        // Fields
        private order _order;
        private string _orderStateImageSource;

        // Properties
        public order Order => _order;
        public string OrderStateImageSource
        {
            get => _orderStateImageSource;
            set
            {
                if (_orderStateImageSource != value)
                {
                    _orderStateImageSource = value;
                    OnPropertyChanged();
                }
            }
        }
        public ActiveSession.OrderState OrderState
        {
            get => (ActiveSession.OrderState)_order.state;
            set
            {
                if (_order.state != (int)value)
                {
                    _order.state = (int)value;
                    OnPropertyChanged();
                }
            }
        }
        public string OrderCode => $"(OC{Order.user_id}{Order.id}{Order.timestamp.DayOfYear})";

        // Ctor
        public OrderDisplay(order orderBase)
        {
            _order = orderBase;

            OrderState = (ActiveSession.OrderState)orderBase.state;
            OrderStateImageSource = $"../../Resources/order_state{orderBase.state}.png";
        }

        public void ChangeOrderState(ActiveSession.OrderState newOrderState)
        {
            if (OrderState == newOrderState) return;

            OrderState = newOrderState;
            OrderStateImageSource = $"../../Resources/order_state{(int)newOrderState}.png";
        }
    }
}
