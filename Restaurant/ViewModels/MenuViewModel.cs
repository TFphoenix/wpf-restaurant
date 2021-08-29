using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Restaurant.Commands;
using Restaurant.Models;
using Restaurant.Models.Services;

namespace Restaurant.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        // Private Fields
        private readonly ProductServices _productServices;
        private ObservableCollection<ProductDisplay> _visibleProducts;
        private List<ProductDisplay> _allProducts;
        private string _searchText = "";

        // Properties
        public ObservableCollection<ProductDisplay> VisibleProducts
        {
            get => _visibleProducts;
            set
            {
                if (_visibleProducts != value)
                {
                    _visibleProducts = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public RelayCommand SearchCommand { get; }

        // Ctor
        public MenuViewModel()
        {
            // Products
            _productServices = new ProductServices();
            _allProducts = new List<ProductDisplay>(_productServices.GetAllProductDisplays());
            VisibleProducts = new ObservableCollection<ProductDisplay>(_allProducts);

            // Commands
            SearchCommand = new RelayCommand(Search);
        }

        private void Search(object param)
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                VisibleProducts = new ObservableCollection<ProductDisplay>(_allProducts);
                return;
            }

            VisibleProducts.Clear();

            // Check search type
            if (_productServices.GetAllergenNames().Any(a => a.Equals(SearchText)))
            {
                // Avoid allergen search type
                foreach (var productDisplay in _allProducts)
                    if (!productDisplay.IsCategoryHeader && !productDisplay.Allergens.Contains(SearchText))
                        VisibleProducts.Add(productDisplay);
            }
            else
            {
                // Product name search type
                foreach (var productDisplay in _allProducts)
                    if (!productDisplay.IsCategoryHeader && productDisplay.Name.Contains(SearchText))
                        VisibleProducts.Add(productDisplay);
            }
        }

        public void AddProductDisplayToBasket(ProductDisplay productDisplay)
        {
            productDisplay.Quantity = 1;
            foreach (var basketProductDisplay in ActiveSession.Basket)
            {
                if (basketProductDisplay.Id == productDisplay.Id)
                {
                    MessageBox.Show("Product already in your basket", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            ActiveSession.Basket.Add(productDisplay);
        }

        public void ReloadProductDisplays()
        {
            _allProducts = new List<ProductDisplay>(_productServices.GetAllProductDisplays());
            VisibleProducts = new ObservableCollection<ProductDisplay>(_allProducts);
        }
    }
}
