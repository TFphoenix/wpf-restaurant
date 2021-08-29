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
using System.Windows.Shapes;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Restaurant.Views.CrudFormViews
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class FormWindow : Window
    {
        public FormWindow(string crudOperation, object operationObject = null)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CrudFormsViewModel.FormWindowRef = this;
            switch (crudOperation)
            {
                case "DishesExp":
                    XExpUc.ViewModelContext.LoadExpirationProductDisplays(true);
                    XExpUc.Visibility = Visibility.Visible;
                    break;
                case "DishesAdd":
                    DishesAddUc.ViewModelContext.LoadCategories();
                    if (operationObject != null)
                        DishesAddUc.ViewModelContext.LoadDishForm(operationObject as dish);
                    DishesAddUc.Visibility = Visibility.Visible;
                    break;
                case "DishesEdit":
                    XEditUc.ViewModelContext.LoadEditableDishesOrMenus(true);
                    XEditUc.Visibility = Visibility.Visible;
                    break;
                case "MenusExp":
                    XExpUc.ViewModelContext.LoadExpirationProductDisplays(false);
                    XExpUc.Visibility = Visibility.Visible;
                    break;
                case "MenusAdd":
                    MenusAddUc.ViewModelContext.LoadCategories();
                    MenusAddUc.ViewModelContext.LoadDishes(operationObject as menu);
                    if (operationObject != null)
                        MenusAddUc.ViewModelContext.LoadMenuForm(operationObject as menu);
                    MenusAddUc.Visibility = Visibility.Visible;
                    break;
                case "MenusEdit":
                    XEditUc.ViewModelContext.LoadEditableDishesOrMenus(false);
                    XEditUc.Visibility = Visibility.Visible;
                    break;
                case "CategoriesEdit":
                    CategoriesEditUc.ViewModelContext.LoadCategories();
                    CategoriesEditUc.Visibility = Visibility.Visible;
                    break;
                case "AllergensEdit":
                    AllergensEditUc.ViewModelContext.LoadAllergens();
                    AllergensEditUc.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No {crudOperation} CRUD Operation defined");
            }
        }
    }
}
