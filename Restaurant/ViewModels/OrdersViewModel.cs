using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Restaurant.Models;
using Restaurant.Models.Services;

namespace Restaurant.ViewModels
{
    class OrdersViewModel : ViewModel
    {
        // Fields
        private readonly OrderServices _orderServices;
        private readonly bool _hasEmployedPrivileges = false;
        private ObservableCollection<OrderDisplay> _orders;

        // Properties
        public ObservableCollection<OrderDisplay> Orders
        {
            get => _orders;
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged();
                }
            }
        }
        public Visibility EmployedVisibility =>
            _hasEmployedPrivileges ? Visibility.Visible : Visibility.Hidden;
        public Visibility CustomerVisibility =>
            _hasEmployedPrivileges ? Visibility.Hidden : Visibility.Visible;

        // Ctor
        public OrdersViewModel()
        {
            // Field initializations
            _orderServices = new OrderServices();
            if (ActiveSession.ConnectedUser != null)
                _hasEmployedPrivileges = ActiveSession.ConnectedUser.role == "employed";
        }

        public void LoadOrdersFromDatabase()
        {
            Orders = _hasEmployedPrivileges ? new ObservableCollection<OrderDisplay>(_orderServices.GetAllOrderDisplays()) : new ObservableCollection<OrderDisplay>(_orderServices.GetUserOrderDisplays(ActiveSession.ConnectedUser.id));
        }

        public void DetailsOnOrder(OrderDisplay orderDisplay)
        {
            if (_hasEmployedPrivileges)
            {
                MessageBox.Show(
                    $"{_orderServices.GetOrderProductsAsString(orderDisplay.Order)}\n{_orderServices.GetOrderCustomerInfoAsString(orderDisplay.Order)}",
                    $"Order Details - {orderDisplay.Order.timestamp}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    $"{_orderServices.GetOrderProductsAsString(orderDisplay.Order)}",
                    $"Order Details - {orderDisplay.Order.timestamp}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ChangeOrderState(OrderDisplay orderDisplay, int newOrderState)
        {
            if (orderDisplay.OrderState == ActiveSession.OrderState.Delivered || orderDisplay.OrderState == ActiveSession.OrderState.Canceled)
            {
                MessageBox.Show("Can't change state of inactive order", "Prohibited action", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBoxResult.Yes;
            if (newOrderState == (int)ActiveSession.OrderState.Delivered || newOrderState == (int)ActiveSession.OrderState.Canceled)
                result = MessageBox.Show(
                "Careful, once you change order state to DELIVERED or CANCELED you won't be able to change it back!\nAre you sure you want to continue?",
                "Triggering inactive order state", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    try
                    {
                        orderDisplay.ChangeOrderState((ActiveSession.OrderState)newOrderState);
                        _orderServices.SetOrderState(orderDisplay.Order.id, newOrderState);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"{e.Message}", "Error changing order state", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case MessageBoxResult.No:
                    return;
                default:
                    return;
            }
            MessageBox.Show($"Order state changed to: {(ActiveSession.OrderState)newOrderState}", "Order state successfully changed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
