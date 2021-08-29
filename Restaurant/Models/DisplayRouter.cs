using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Restaurant.Views;

namespace Restaurant.Models
{
    public static class DisplayRouter
    {
        public static StartupWindow StartupWindowReference;
        public static AppWindow AppWindowReference;

        // WINDOW NAVIGATION
        public static void SwitchToAppWindow()
        {
            new AppWindow().Show();
            StartupWindowReference.Close();
        }

        public static void SwitchToStartupWindow()
        {
            new StartupWindow().Show();
            AppWindowReference.Close();
        }

        // USER CONTROL NAVIGATION - StartupWindow
        public static void DisplaySignUp()
        {
            StartupWindowReference.SignInView.Visibility = Visibility.Hidden;
            StartupWindowReference.SignUpView.Visibility = Visibility.Visible;
        }

        public static void DisplaySignIn()
        {
            StartupWindowReference.SignUpView.Visibility = Visibility.Hidden;
            StartupWindowReference.SignInView.Visibility = Visibility.Visible;
        }

        // USER CONTROL NAVIGATION - AppWindow
        public enum AppTab
        {
            Menu,
            Orders,
            Products,
            Basket
        }

        public static void DisplayAppTab(AppTab appTab)
        {
            switch (appTab)
            {
                case AppTab.Menu:
                    AppWindowReference.BasketView.Visibility = Visibility.Hidden;
                    AppWindowReference.ProductsView.Visibility = Visibility.Hidden;
                    AppWindowReference.OrdersView.Visibility = Visibility.Hidden;
                    AppWindowReference.MenuView.Visibility = Visibility.Visible;
                    break;
                case AppTab.Orders:
                    AppWindowReference.BasketView.Visibility = Visibility.Hidden;
                    AppWindowReference.ProductsView.Visibility = Visibility.Hidden;
                    AppWindowReference.MenuView.Visibility = Visibility.Hidden;
                    AppWindowReference.OrdersView.ViewModelContext.LoadOrdersFromDatabase();
                    AppWindowReference.OrdersView.Visibility = Visibility.Visible;
                    break;
                case AppTab.Products:
                    AppWindowReference.BasketView.Visibility = Visibility.Hidden;
                    AppWindowReference.OrdersView.Visibility = Visibility.Hidden;
                    AppWindowReference.MenuView.Visibility = Visibility.Hidden;
                    AppWindowReference.ProductsView.Visibility = Visibility.Visible;
                    break;
                case AppTab.Basket:
                    AppWindowReference.MenuView.Visibility = Visibility.Hidden;
                    AppWindowReference.OrdersView.Visibility = Visibility.Hidden;
                    AppWindowReference.ProductsView.Visibility = Visibility.Hidden;
                    AppWindowReference.BasketView.ViewModelContext.LoadFromActiveSessionBasket();
                    AppWindowReference.BasketView.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(appTab), appTab, null);
            }
        }

        public static void ReloadProductDisplays()
        {
            AppWindowReference.MenuView.ViewModelContext.ReloadProductDisplays();
        }

    }
}
