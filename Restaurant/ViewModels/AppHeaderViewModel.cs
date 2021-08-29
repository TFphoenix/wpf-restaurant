using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Restaurant.Commands;
using Restaurant.Models;

namespace Restaurant.ViewModels
{
    public class AppHeaderViewModel : ViewModel
    {
        private string _userName;
        private Visibility _userVisibility;

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }
        public Visibility UserVisibility
        {
            get => _userVisibility;
            set
            {
                if (_userVisibility != value)
                {
                    _userVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        public Visibility NonUserVisibility => UserVisibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

        public RelayCommand LogoutCommand { get; }
        public RelayCommand MenuTabCommand { get; }
        public RelayCommand OrdersTabCommand { get; }
        public RelayCommand ProductsTabCommand { get; }
        public RelayCommand BasketTabCommand { get; }

        public AppHeaderViewModel()
        {
            // Connected User Details
            if (ActiveSession.ConnectedUser != null)
            {
                UserVisibility = Visibility.Visible;
                UserName = ActiveSession.ConnectedUser.first_name + " " + ActiveSession.ConnectedUser.last_name;
            }
            else
            {
                UserVisibility = Visibility.Hidden;
            }

            // Commands
            LogoutCommand = new RelayCommand(Logout);
            MenuTabCommand = new RelayCommand(ChangeTab);
            OrdersTabCommand = new RelayCommand(ChangeTab, CanAccessTabUser);
            ProductsTabCommand = new RelayCommand(ChangeTab, CanAccessTabEmployed);
            BasketTabCommand = new RelayCommand(ChangeTab, CanAccessTabCustomer);
        }

        private void ChangeTab(object param)
        {
            switch ((string)((Button)param).Content)
            {
                case "Menu":
                    DisplayRouter.DisplayAppTab(DisplayRouter.AppTab.Menu);
                    break;
                case "Orders":
                    DisplayRouter.DisplayAppTab(DisplayRouter.AppTab.Orders);
                    break;
                case "Products":
                    DisplayRouter.DisplayAppTab(DisplayRouter.AppTab.Products);
                    break;
                case "Basket":
                    DisplayRouter.DisplayAppTab(DisplayRouter.AppTab.Basket);
                    break;
                default:
                    return;
            }
        }

        private void Logout(object param)
        {
            if ((string)((Button)param).Content == "Logout")
                ActiveSession.ConnectedUser = null;
            DisplayRouter.SwitchToStartupWindow();
        }

        private bool CanAccessTabEmployed(object param)
        {
            return ActiveSession.ConnectedUser != null && ActiveSession.ConnectedUser.role == "employed";
        }

        private bool CanAccessTabCustomer(object param)
        {
            return ActiveSession.ConnectedUser != null && ActiveSession.ConnectedUser.role == "customer";
        }

        private bool CanAccessTabUser(object param)
        {
            return ActiveSession.ConnectedUser != null;
        }
    }
}
