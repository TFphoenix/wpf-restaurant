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

namespace Restaurant.Views.CrudFormViews
{
    /// <summary>
    /// Interaction logic for XEdit.xaml
    /// </summary>
    public partial class XEdit : UserControl
    {
        public XEdit()
        {
            InitializeComponent();
        }

        // Dish Actions
        private void EditDish_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dish dishToEdit = (sender as FrameworkElement).DataContext as dish;
                ViewModelContext.EditDish(dishToEdit);
            }
            catch (Exception)
            {
                // empty
            }
        }

        private void DeleteDish_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dish dishToDelete = (sender as FrameworkElement).DataContext as dish;
                ViewModelContext.DeleteDish(dishToDelete);
            }
            catch (Exception)
            {
                // empty
            }
        }

        // Menu Actions
        private void EditMenu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                menu menuToEdit = (sender as FrameworkElement).DataContext as menu;
                ViewModelContext.EditMenu(menuToEdit);
            }
            catch (Exception)
            {
                // empty
            }
        }

        private void DeleteMenu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                menu menuToDelete = (sender as FrameworkElement).DataContext as menu;
                ViewModelContext.DeleteMenu(menuToDelete);
            }
            catch (Exception)
            {
                // empty
            }
        }
    }
}
