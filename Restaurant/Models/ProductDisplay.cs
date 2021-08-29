using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Restaurant.ViewModels;

namespace Restaurant.Models
{
    public class ProductDisplay : ViewModel
    {
        private string _imageSource;
        private int _imageIndex = 0;
        private bool _isAvailable;
        private bool _isCategoryHeader;
        private double _price;
        private int _quantity;
        private double _quantityPrice;

        // Product Properties
        private List<string> ImageSourceList;
        public int Id { get; set; }
        public bool IsMenu { get; set; }
        public string ImageSource
        {
            get => ImageSourceList == null ? "../Resources/logo_min.png" : ImageSourceList[_imageIndex];
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Name { get; set; }
        public string Allergens { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price
        {
            get => Math.Round(_price, 2);
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    QuantityPrice = Quantity * Price;
                    OnPropertyChanged();
                }
            }
        }
        public double QuantityPrice
        {
            get => Math.Round(_quantityPrice, 2);
            set
            {
                if (_quantityPrice != value)
                {
                    _quantityPrice = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        // Display Properties
        public Visibility NonAvailabilityVisibility => IsAvailable ? Visibility.Hidden : Visibility.Visible;
        public Visibility ConnectedUserVisibility =>
            ActiveSession.ConnectedUser == null || ActiveSession.ConnectedUser.role != "customer" ? Visibility.Hidden : Visibility.Visible;
        public bool IsCategoryHeader
        {
            get => _isCategoryHeader;
            set
            {
                if (_isCategoryHeader != value)
                {
                    _isCategoryHeader = value;
                    OnPropertyChanged();
                }
            }
        }
        public Visibility IsNormalProductVisibility => IsCategoryHeader ? Visibility.Hidden : Visibility.Visible;
        public Visibility IsCategoryHeaderVisibility => IsCategoryHeader ? Visibility.Visible : Visibility.Hidden;

        public ProductDisplay()
        {
            Name = "";
            Allergens = "";
            Description = "";
            Category = "";
            Price = 0;
            Quantity = 0;
            IsAvailable = true;
            IsCategoryHeader = false;
        }

        public void NextImage()
        {
            ++_imageIndex;
            if (_imageIndex >= ImageSourceList.Count)
                _imageIndex = 0;
            ImageSource = ImageSourceList[_imageIndex];
        }

        public void PreviousImage()
        {
            --_imageIndex;
            if (_imageIndex < 0)
                _imageIndex = ImageSourceList.Count - 1;
            ImageSource = ImageSourceList[_imageIndex];
        }

        public void PopulateImageSourceList(string PhotosSource, int PhotosCount)
        {
            if (PhotosCount == 0) return;

            try
            {
                ImageSourceList = new List<string>();
                for (int i = 0; i < PhotosCount; i++)
                {
                    ImageSourceList.Add($"../Resources/ProductPhotos/{PhotosSource}/{i}.png");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
