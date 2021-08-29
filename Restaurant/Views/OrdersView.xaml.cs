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
    /// Interaction logic for OrdersView.xaml
    /// </summary>
    public partial class OrdersView : UserControl
    {
        public OrdersView()
        {
            InitializeComponent();
        }

        private void Details_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var orderDisplay = (sender as FrameworkElement).DataContext as OrderDisplay;
                ViewModelContext.DetailsOnOrder(orderDisplay);
            }
            catch
            {
                // empty
            }
        }

        private void ChangeState_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int changeToState = (sender as Button).Name[1] - '0';
                var orderDisplay = (sender as FrameworkElement).DataContext as OrderDisplay;
                ViewModelContext.ChangeOrderState(orderDisplay, changeToState);
            }
            catch
            {
                // empty
            }
        }
    }
}
