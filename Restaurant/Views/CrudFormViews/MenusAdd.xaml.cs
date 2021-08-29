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
    /// Interaction logic for MenusAdd.xaml
    /// </summary>
    public partial class MenusAdd : UserControl
    {
        public MenusAdd()
        {
            InitializeComponent();
        }

        private void RemoveMenuDish_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuDishToRemove = (sender as FrameworkElement).DataContext as menu_dish;
                ViewModelContext.RemoveMenuDishFromMenu(menuDishToRemove);
            }
            catch (Exception)
            {
                // empty
            }
        }
    }
}
