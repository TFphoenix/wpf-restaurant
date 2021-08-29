using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Restaurant.Models;

namespace Restaurant.Views
{
    /// <summary>
    /// Interaction logic for BasketView.xaml
    /// </summary>
    public partial class BasketView : UserControl
    {
        public BasketView()
        {
            InitializeComponent();
        }

        private void PreviousImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var productDisplay = (sender as FrameworkElement).DataContext as ProductDisplay;
                productDisplay.PreviousImage();
            }
            catch
            {
                // empty
            }
        }

        private void NextImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var productDisplay = (sender as FrameworkElement).DataContext as ProductDisplay;
                productDisplay.NextImage();
            }
            catch
            {
                // empty
            }
        }

        private void Minus_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var productDisplay = (sender as FrameworkElement).DataContext as ProductDisplay;
                if (productDisplay.Quantity > 1)
                    productDisplay.Quantity--;
                ViewModelContext.UpdatePrices();
            }
            catch
            {
                // empty
            }
        }

        private void Plus_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var productDisplay = (sender as FrameworkElement).DataContext as ProductDisplay;
                if (productDisplay.Quantity < 10)
                    productDisplay.Quantity++;
                ViewModelContext.UpdatePrices();
            }
            catch
            {
                // empty
            }
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var productDisplay = (sender as FrameworkElement).DataContext as ProductDisplay;
                ViewModelContext.RemoveProductDisplayById(productDisplay.Id);
            }
            catch
            {
                // empty
            }
        }
    }
}
