using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Restaurant.Commands;
using Restaurant.Models;
using Restaurant.Models.Services;
using Restaurant.Views.CrudFormViews;

namespace Restaurant.ViewModels
{
    public class CrudFormsViewModel : ViewModel
    {
        // Form Classes
        public class DishForm : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public bool Editing { get; set; } = false;
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public category Category { get; set; }
            public string Allergens { get; set; } = "";
            public int PhotosCount { get; set; }
            public string MeasurementUnit { get; set; }
            public double Price { get; set; }
            public int PortionQuantity { get; set; }
            public int TotalQuantity { get; set; }
        }
        public class MenuForm : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public bool Editing { get; set; } = false;
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public category Category { get; set; }
            public int PhotosCount { get; set; }
            public dish Dish { get; set; }
            public string Quantity { get; set; }
        }

        // Fields
        private readonly ProductServices _productServices;
        private ObservableCollection<ProductDisplay> _expirationProductDisplays = new ObservableCollection<ProductDisplay>();
        private ObservableCollection<category> _categories = new ObservableCollection<category>();
        private ObservableCollection<menu_dish> _menuDishes = new ObservableCollection<menu_dish>();
        private ObservableCollection<menu> _menus = new ObservableCollection<menu>();
        private ObservableCollection<dish> _dishes = new ObservableCollection<dish>();
        private readonly List<category> _categoriesToDelete = new List<category>();
        private readonly List<allergen> _allergensToDelete = new List<allergen>();

        // References
        public static FormWindow FormWindowRef { get; set; }
        public static FormWindow PreviousFormWindowRef { get; set; }

        // Properties
        public int ProductsExpirationCount => Properties.Settings.Default.ProductsExpirationCount;
        public ObservableCollection<ProductDisplay> ExpirationProductDisplays
        {
            get => _expirationProductDisplays;
            set
            {
                if (_expirationProductDisplays != value)
                {
                    _expirationProductDisplays = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<int> ProductPhotosCount { get; set; } = new ObservableCollection<int>(new List<int>()
        {
            1, 2, 3, 4, 5
        });
        public ObservableCollection<string> ProductMeasurementUnits { get; set; } = new ObservableCollection<string>(new List<string>()
        {
            "g", "ml", "piece", "pack"
        });
        public DishForm DishFormObject { get; set; }
        public MenuForm MenuFormObject { get; set; }
        public ObservableCollection<dish> Dishes
        {
            get => _dishes;
            set
            {
                if (_dishes != value)
                {
                    _dishes = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<menu_dish> MenuDishes
        {
            get => _menuDishes;
            set
            {
                if (_menuDishes != value)
                {
                    _menuDishes = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<menu> Menus
        {
            get => _menus;
            set
            {
                if (_menus != value)
                {
                    _menus = value;
                    OnPropertyChanged();
                }
            }
        }
        public menu_dish SelectedMenuDish { get; set; }
        public category SelectedCategory { get; set; }
        public ObservableCollection<allergen> Allergens { get; set; }
        public allergen SelectedAllergen { get; set; }
        public Visibility EditingDishesVisibility { get; set; } = Visibility.Hidden;
        public Visibility EditingMenusVisibility { get; set; } = Visibility.Hidden;

        // Commands
        public RelayCommand CommitChangesDishCommand { get; }
        public RelayCommand CommitChangesMenuCommand { get; }
        public RelayCommand CommitChangesCategoryCommand { get; }
        public RelayCommand AddDishToMenuCommand { get; }
        public RelayCommand RemoveCategoryCommand { get; }
        public RelayCommand RemoveAllergenCommand { get; }
        public RelayCommand CommitChangesAllergenCommand { get; }

        //Ctor
        public CrudFormsViewModel()
        {
            // Fields init.
            _productServices = new ProductServices();

            // Properties init.
            DishFormObject = new DishForm();
            MenuFormObject = new MenuForm();

            // Commands init.
            CommitChangesDishCommand = new RelayCommand(CommitChangesDish);
            CommitChangesMenuCommand = new RelayCommand(CommitChangesMenu);
            AddDishToMenuCommand = new RelayCommand(AddDishToMenu, CanAddDishToMenu);
            CommitChangesCategoryCommand = new RelayCommand(CommitChangesCategory);
            RemoveCategoryCommand = new RelayCommand(RemoveSelectedCategory, CanRemoveSelectedCategory);
            RemoveAllergenCommand = new RelayCommand(RemoveSelectedAllergen, CanRemoveSelectedAllergen);
            CommitChangesAllergenCommand = new RelayCommand(CommitChangesAllergen);
        }


        // Action Methods
        private bool CanRemoveSelectedAllergen(object obj)
        {
            return SelectedAllergen != null;
        }

        private void RemoveSelectedAllergen(object obj)
        {
            var areYouSure = MessageBox.Show(
                $"Are you sure you want to delete allergen: {SelectedAllergen.name}?\nDELETING THIS ALLERGEN WILL ALSO DELETE ALL PRODUCTS ASOCIATED WITH IT",
                "Deleting allergen...", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            switch (areYouSure)
            {
                case MessageBoxResult.Yes:
                    _allergensToDelete.Add(SelectedAllergen);
                    Allergens.Remove(SelectedAllergen);
                    break;
                default:
                    return;
            }
        }

        private bool CanRemoveSelectedCategory(object obj)
        {
            return SelectedCategory != null;
        }

        private void RemoveSelectedCategory(object obj)
        {
            var areYouSure = MessageBox.Show(
                $"Are you sure you want to delete category: {SelectedCategory.name}?\nDELETING THIS CATEGORY WILL ALSO DELETE ALL PRODUCTS ASOCIATED WITH IT",
                "Deleting category...", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            switch (areYouSure)
            {
                case MessageBoxResult.Yes:
                    _categoriesToDelete.Add(SelectedCategory);
                    Categories.Remove(SelectedCategory);
                    break;
                default:
                    return;
            }
        }

        public void RemoveMenuDishFromMenu(menu_dish menuDishToRemove)
        {
            var aux = new ObservableCollection<menu_dish>(MenuDishes);
            aux.Remove(menuDishToRemove);
            MenuDishes = aux;
            Dishes.Add(menuDishToRemove.dish);
        }

        private void AddDishToMenu(object obj)
        {
            var aux = new ObservableCollection<menu_dish>(MenuDishes);
            aux.Add(new menu_dish()
            {
                dish = MenuFormObject.Dish,
                quantity = int.Parse(MenuFormObject.Quantity)
            });
            MenuDishes = aux;
            Dishes.Remove(MenuFormObject.Dish);
            MenuFormObject.Quantity = "";
        }

        private bool CanAddDishToMenu(object obj)
        {
            try
            {
                return MenuFormObject.Dish != null &&
                    int.Parse(MenuFormObject.Quantity) != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Commit Methods
        private void CommitChangesAllergen(object obj)
        {
            try
            {
                _productServices.CommitAllergens(Allergens.ToList(), _allergensToDelete);
                _allergensToDelete.Clear();
                LoadAllergens();
                DisplayRouter.ReloadProductDisplays();
                MessageBox.Show($"Succesfully commited changes on allergens", "Changes commited", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CommitChangesCategory(object obj)
        {
            try
            {
                _productServices.CommitCategories(Categories.ToList(), _categoriesToDelete);
                _categoriesToDelete.Clear();
                LoadCategories();
                DisplayRouter.ReloadProductDisplays();
                MessageBox.Show($"Succesfully commited changes on categories", "Changes commited", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CommitChangesDish(object obj)
        {
            if (string.IsNullOrEmpty(DishFormObject.Name) ||
                (int)DishFormObject.Price == 0 ||
                  DishFormObject.PortionQuantity == 0 ||
                  DishFormObject.TotalQuantity == 0 ||
                  DishFormObject.Category == null ||
                string.IsNullOrEmpty(DishFormObject.MeasurementUnit) ||
                DishFormObject.PhotosCount == 0)
            {
                MessageBox.Show("Dish property fields wrongly filled", "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                _productServices.CommitDishFromForm(DishFormObject);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            DisplayRouter.ReloadProductDisplays();
            MessageBox.Show($"Succesfully commited changes on dish: {DishFormObject.Name}", "Changes commited", MessageBoxButton.OK, MessageBoxImage.Information);
            FormWindowRef.Close();
        }

        private void CommitChangesMenu(object obj)
        {
            if (string.IsNullOrEmpty(MenuFormObject.Name) ||
                MenuFormObject.Category == null ||
                MenuDishes.Count < 2 ||
                MenuFormObject.PhotosCount == 0)
            {
                MessageBox.Show("Menu property fields wrongly filled", "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                _productServices.CommitMenuFromForm(MenuFormObject, MenuDishes.ToList());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error commiting changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            DisplayRouter.ReloadProductDisplays();
            MessageBox.Show($"Succesfully commited changes on menu: {MenuFormObject.Name}", "Changes commited",
                MessageBoxButton.OK, MessageBoxImage.Information);
            FormWindowRef.Close();
        }

        public void DeleteDish(dish dishToDelete)
        {
            var areYouSure = MessageBox.Show(
                $"Are you sure you want to permanently delete dish: {dishToDelete.product.name}?\nCAUTION, THIS WILL ALSO DELETE ALL MENUS ASSOCIATED WITH IT",
                "Dish removal", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            switch (areYouSure)
            {
                case MessageBoxResult.Yes:
                    _productServices.DeleteDish(dishToDelete);
                    DisplayRouter.ReloadProductDisplays();
                    LoadDishes();
                    break;
                default:
                    return;
            }
        }

        public void DeleteMenu(menu menuToDelete)
        {
            var areYouSure = MessageBox.Show(
                $"Are you sure you want to permanently delete menu: {menuToDelete.product.name}?",
                "Menu removal", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            switch (areYouSure)
            {
                case MessageBoxResult.Yes:
                    _productServices.DeleteMenu(menuToDelete);
                    DisplayRouter.ReloadProductDisplays();
                    LoadMenus();
                    break;
                default:
                    return;
            }
        }

        public void EditDish(dish dishToEdit)
        {
            PreviousFormWindowRef = FormWindowRef;
            new FormWindow("DishesAdd", dishToEdit).ShowDialog();
            PreviousFormWindowRef.Close();
            new FormWindow("DishesEdit").ShowDialog();
        }

        public void EditMenu(menu menuToEdit)
        {
            PreviousFormWindowRef = FormWindowRef;
            new FormWindow("MenusAdd", menuToEdit).ShowDialog();
            PreviousFormWindowRef.Close();
            new FormWindow("MenusEdit").ShowDialog();
        }

        // Loading Methods
        public void LoadExpirationProductDisplays(bool forDishes)
        {
            ExpirationProductDisplays = forDishes ?
                new ObservableCollection<ProductDisplay>(_productServices.GetExpirationDishProductDisplays()) :
                new ObservableCollection<ProductDisplay>(_productServices.GetExpirationMenuProductDisplays());
        }

        public void LoadCategories()
        {
            Categories = new ObservableCollection<category>(_productServices.GetAllCategories());
        }

        public void LoadDishes(menu menuItem = null)
        {
            var allDishes = _productServices.GetAllDishes();
            if (menuItem == null)
            {
                // Add Menu
                Dishes = new ObservableCollection<dish>(allDishes);
            }
            else
            {
                // Edit Menu
                MenuDishes = new ObservableCollection<menu_dish>(_productServices.GetMenuDishes(menuItem));
                Dishes = new ObservableCollection<dish>(allDishes);
                foreach (var menuDish in MenuDishes)
                {
                    Dishes.Remove(menuDish.dish);
                }
            }
        }

        public void LoadMenus()
        {
            Menus = new ObservableCollection<menu>(_productServices.GetAllMenus());
        }

        public void LoadAllergens()
        {
            Allergens = new ObservableCollection<allergen>(_productServices.GetAllAllergens());
        }

        public void LoadEditableDishesOrMenus(bool editingDishes)
        {
            if (editingDishes)
            {
                LoadDishes();
                EditingMenusVisibility = Visibility.Hidden;
                EditingDishesVisibility = Visibility.Visible;
            }
            else
            {
                LoadMenus();
                EditingDishesVisibility = Visibility.Hidden;
                EditingMenusVisibility = Visibility.Visible;
            }
        }

        public void LoadMenuForm(menu menuToLoadFrom)
        {
            MenuFormObject.Editing = true;
            MenuFormObject.Id = menuToLoadFrom.id;
            MenuFormObject.Name = menuToLoadFrom.product.name;
            MenuFormObject.PhotosCount = menuToLoadFrom.product.photos_count ?? 1;
            // Category
            foreach (var categoryItem in Categories)
            {
                if (categoryItem.code == menuToLoadFrom.product.category_code)
                {
                    MenuFormObject.Category = categoryItem;
                    break;
                }
            }
        }

        public void LoadDishForm(dish dishToLoadFrom)
        {
            DishFormObject.Editing = true;
            DishFormObject.Id = dishToLoadFrom.id;
            DishFormObject.Name = dishToLoadFrom.product.name;
            DishFormObject.PhotosCount = dishToLoadFrom.product.photos_count ?? 1;
            DishFormObject.MeasurementUnit = dishToLoadFrom.measurement_unit;
            DishFormObject.Price = dishToLoadFrom.price;
            DishFormObject.PortionQuantity = dishToLoadFrom.portion_quantity;
            DishFormObject.TotalQuantity = dishToLoadFrom.total_quantity;

            // Allergens
            DishFormObject.Allergens = "";
            foreach (var dishAllergen in dishToLoadFrom.dish_allergen)
            {
                DishFormObject.Allergens += dishAllergen.allergen.name + ",";
            }
            if (!string.IsNullOrEmpty(DishFormObject.Allergens))
                DishFormObject.Allergens = DishFormObject.Allergens.Remove(DishFormObject.Allergens.Length - 1);

            // Category
            foreach (var categoryItem in Categories)
            {
                if (categoryItem.code == dishToLoadFrom.product.category_code)
                {
                    DishFormObject.Category = categoryItem;
                    break;
                }
            }
        }
    }
}
