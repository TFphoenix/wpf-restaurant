using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Restaurant.Commands;
using Restaurant.Models;
using Restaurant.Models.Services;

namespace Restaurant.ViewModels
{
    public class BasketViewModel : ViewModel
    {
        // Fields
        private ObservableCollection<ProductDisplay> _basketProducts;
        private readonly OrderServices _orderServices;
        private double _discount;
        private double _deliveryCost;
        private double _orderPrice;
        private double _totalOrderPrice;

        // Properties
        public ObservableCollection<ProductDisplay> BasketProducts
        {
            get => _basketProducts;
            set
            {
                if (_basketProducts != value)
                {
                    _basketProducts = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Discount
        {
            get => Math.Round(_discount, 2);
            set
            {
                if (_discount != value)
                {
                    _discount = value;
                    OnPropertyChanged();
                }
            }
        }
        public double DeliveryCost
        {
            get => Math.Round(_deliveryCost, 2);
            set
            {
                if (_deliveryCost != value)
                {
                    _deliveryCost = value;
                    OnPropertyChanged();
                }
            }
        }
        public double TotalOrderPrice
        {
            get => Math.Round(_totalOrderPrice, 2);
            set
            {
                if (_totalOrderPrice != value)
                {
                    _totalOrderPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public RelayCommand PlaceOrderCommand { get; set; }
        public RelayCommand ClearAllCommand { get; set; }

        // Ctor
        public BasketViewModel()
        {
            // Order Services
            _orderServices = new OrderServices();

            // Property initializations
            BasketProducts = new ObservableCollection<ProductDisplay>();
            Discount = TotalOrderPrice = DeliveryCost = 0;

            // Command initializations
            PlaceOrderCommand = new RelayCommand(PlaceOrder, ListNotEmpty);
            ClearAllCommand = new RelayCommand(ClearAll, ListNotEmpty);
        }

        public void SaveToActiveSessionBasket()
        {
            ActiveSession.Basket = BasketProducts.ToList();
        }

        public void LoadFromActiveSessionBasket()
        {
            BasketProducts = new ObservableCollection<ProductDisplay>(ActiveSession.Basket);
            UpdatePrices();
        }

        private void PlaceOrder(object param)
        {
            try
            {
                SaveToActiveSessionBasket();
                _orderServices.PlaceOrder(Discount, DeliveryCost, TotalOrderPrice);
                ClearAll(new object());
                DisplayRouter.ReloadProductDisplays();
                MessageBox.Show(
                    "Your order has been registered and it will be shortly delivered to you.\nThanks for choosing DORU's Restaurant!",
                    "Order successfully placed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentOutOfRangeException e)
            {
                MessageBox.Show(e.ParamName, "Error placing order", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ClearAll(object param)
        {
            BasketProducts.Clear();
            SaveToActiveSessionBasket();
            UpdatePrices();
        }

        private bool ListNotEmpty(object param)
        {
            return BasketProducts.Count != 0;
        }

        public void RemoveProductDisplayById(int id)
        {
            for (int index = 0; index < BasketProducts.Count; index++)
            {
                if (BasketProducts[index].Id == id)
                {
                    BasketProducts.RemoveAt(index);
                    SaveToActiveSessionBasket();
                    UpdatePrices();
                    return;
                }
            }
        }

        public void UpdatePrices()
        {
            // Empty Basket
            if (BasketProducts.Count == 0)
            {
                _orderPrice = DeliveryCost = Discount = TotalOrderPrice = 0;
                return;
            }

            // Order Price
            _orderPrice = 0;
            foreach (var productDisplay in BasketProducts)
            {
                _orderPrice += productDisplay.QuantityPrice;
            }

            // Delivery Cost
            DeliveryCost = Properties.Settings.Default.DeliveryPrice;
            if (_orderPrice > Properties.Settings.Default.DeliveryPricePoint)
            {
                DeliveryCost = 0;
            }

            // Discounts
            Discount = 0;
            if (_orderPrice > Properties.Settings.Default.OrderPricePoint)
            {
                Discount = _orderPrice * Properties.Settings.Default.OrderDiscount;
            }
            else try
                {
                    var timeSinceLastOrder = DateTime.Now - _orderServices.GetUserLastOrder(ActiveSession.ConnectedUser.id).timestamp;
                    if (timeSinceLastOrder < Properties.Settings.Default.OrderTimeInterval)
                    {
                        Discount = _orderPrice * Properties.Settings.Default.OrderDiscount;
                    }
                }
                catch (Exception)
                {
                    // empty
                }

            // Total Order Price
            TotalOrderPrice = _orderPrice + DeliveryCost - Discount;
        }
    }
}
