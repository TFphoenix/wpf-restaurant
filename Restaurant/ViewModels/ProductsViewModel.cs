using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Restaurant.Commands;
using Restaurant.Views.CrudFormViews;

namespace Restaurant.ViewModels
{
    public class ProductsViewModel : ViewModel
    {
        public RelayCommand CrudCommand { get; }

        public ProductsViewModel()
        {
            CrudCommand = new RelayCommand(OpenCrudWindow);
        }

        private void OpenCrudWindow(object param)
        {
            new FormWindow((param as Button).Name).ShowDialog();
        }
    }
}
