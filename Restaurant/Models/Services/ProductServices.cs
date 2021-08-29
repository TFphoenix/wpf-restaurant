using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Restaurant.ViewModels;

namespace Restaurant.Models.Services
{
    class ProductServices
    {
        private readonly RestaurantEDM _context = new RestaurantEDM();

        public List<ProductDisplay> GetAllProductDisplays()
        {
            List<ProductDisplay> productDisplays = new List<ProductDisplay>();

            // Categories
            foreach (var categoryType in _context.categories)
            {
                // Category Header
                productDisplays.Add(new ProductDisplay
                {
                    IsCategoryHeader = true,
                    Category = categoryType.name
                });

                // Products
                var productList = _context.spGetProductsByCategory_safe(categoryType.code).ToList();
                foreach (var productItem in productList)
                {
                    //Product Display
                    ProductDisplay productDisplay = new ProductDisplay();

                    // Id, Name & Category
                    productDisplay.Id = productItem.id;
                    productDisplay.IsMenu = productItem.is_menu;
                    productDisplay.Name = productItem.name;
                    productDisplay.Category = categoryType.name;

                    // Description, Price & IsAvailable
                    if (productItem.is_menu)
                    {
                        var menuDishes = _context.spGetMenuDishesByProductId_safe(productItem.id).ToList();
                        foreach (var dishItem in menuDishes)
                        {
                            var dishProduct =
                                _context.products.Where(p => p.id.Equals(dishItem.product_id)).ToList()[0];
                            productDisplay.Description += $"{dishProduct.name}({dishItem.quantity}{dishItem.measurement_unit}), ";
                            productDisplay.Price += dishItem.price;
                            if (dishItem.total_quantity < dishItem.quantity) productDisplay.IsAvailable = false;
                        }

                        // Menu Discount
                        productDisplay.Price -= productDisplay.Price * Properties.Settings.Default.MenuDiscount;

                        if (!String.IsNullOrEmpty(productDisplay.Description))
                            productDisplay.Description = productDisplay.Description.Remove(productDisplay.Description.Length - 2);
                    }
                    else
                    {
                        var dishItem = _context.dishes.Where(d => d.product_id.Equals(productItem.id)).ToList()[0];
                        productDisplay.Description = $"{dishItem.portion_quantity}{dishItem.measurement_unit}";
                        productDisplay.Price = dishItem.price;
                        if (dishItem.total_quantity < dishItem.portion_quantity) productDisplay.IsAvailable = false;
                    }

                    // Allergens
                    var itemAllergens = _context.spGetProductAllergens(productItem.id).ToList();
                    foreach (var allg in itemAllergens) productDisplay.Allergens += $"{allg}, ";
                    if (!String.IsNullOrEmpty(productDisplay.Allergens))
                        productDisplay.Allergens = productDisplay.Allergens.Remove(productDisplay.Allergens.Length - 2);
                    else
                        productDisplay.Allergens = "-";

                    // ImageSourceList
                    productDisplay.PopulateImageSourceList(productItem.photos_source, productItem.photos_count ?? 0);

                    // Add Product Diplay to List
                    productDisplays.Add(productDisplay);
                }
            }

            return productDisplays;
        }

        public List<string> GetAllergenNames()
        {
            List<string> allergenNames = new List<string>();

            foreach (var contextAllergen in _context.allergens)
            {
                allergenNames.Add(contextAllergen.name);
            }

            return allergenNames;
        }

        public List<ProductDisplay> GetExpirationDishProductDisplays()
        {
            List<ProductDisplay> productDisplays = new List<ProductDisplay>();

            foreach (var dish in _context.dishes)
            {
                var availability = _context.spGetProductAvailability(dish.product_id).First();
                if (availability < Properties.Settings.Default.ProductsExpirationCount)
                {
                    productDisplays.Add(new ProductDisplay()
                    {
                        Name = dish.product.name,
                        Category = dish.product.category.name,
                        Price = dish.price,
                        IsAvailable = availability != 0,
                        Quantity = availability ?? 0
                    });
                }
            }

            return productDisplays;
        }

        public List<ProductDisplay> GetExpirationMenuProductDisplays()
        {
            List<ProductDisplay> productDisplays = new List<ProductDisplay>();

            foreach (var menu in _context.menus)
            {
                var availability = _context.spGetProductAvailability(menu.product_id).First();
                if (availability < Properties.Settings.Default.ProductsExpirationCount)
                {
                    productDisplays.Add(new ProductDisplay()
                    {
                        Name = menu.product.name,
                        Category = menu.product.category.name,
                        Price = _context.spGetMenuPriceByMenuId(menu.id).First() ?? 0,
                        IsAvailable = availability != 0,
                        Quantity = availability ?? 0
                    });
                }
            }

            return productDisplays;
        }

        public List<category> GetAllCategories()
        {
            return _context.categories.ToList();
        }

        public List<dish> GetAllDishes()
        {
            return _context.dishes.ToList();
        }

        public List<menu> GetAllMenus()
        {
            return _context.menus.ToList();
        }

        public List<allergen> GetAllAllergens()
        {
            return _context.allergens.ToList();
        }

        public List<menu_dish> GetMenuDishes(menu menuItem)
        {
            return (from md in _context.menu_dish
                    where md.menu_id == menuItem.id
                    select md).ToList();
        }

        public void CommitDishFromForm(CrudFormsViewModel.DishForm dishForm)
        {
            // Editing vs Adding
            product productToCommit;
            dish dishToCommit;
            if (dishForm.Editing)
            {
                dishToCommit = (from d in _context.dishes
                                where d.id == dishForm.Id
                                select d).First();
                productToCommit = dishToCommit.product;
            }
            else
            {
                productToCommit = new product();
                dishToCommit = new dish();
            }

            // Change properties
            productToCommit.name = dishForm.Name;
            productToCommit.category = dishForm.Category;
            productToCommit.photos_source = dishForm.Name;
            productToCommit.photos_count = dishForm.PhotosCount;
            productToCommit.is_menu = false;

            dishToCommit.product = productToCommit;
            dishToCommit.measurement_unit = dishForm.MeasurementUnit;
            dishToCommit.price = dishForm.Price;
            dishToCommit.portion_quantity = dishForm.PortionQuantity;
            dishToCommit.total_quantity = dishForm.TotalQuantity;

            //Find old da
            List<int> dishAllergensToRemoveIds = new List<int>();
            foreach (var dishAllergen in dishToCommit.dish_allergen)
            {
                dishAllergensToRemoveIds.Add(dishAllergen.id);
            }

            // Remove old da
            foreach (var id in dishAllergensToRemoveIds)
            {
                var dishAllergenToRemove = (from da in _context.dish_allergen
                                            where da.id == id
                                            select da).First();
                _context.dish_allergen.Remove(dishAllergenToRemove);
            }

            // Add new da
            if (!string.IsNullOrEmpty(dishForm.Allergens))
            {
                List<string> allergenList = dishForm.Allergens.Split(',').ToList();
                foreach (var allergenName in allergenList)
                {
                    try
                    {
                        dishToCommit.dish_allergen.Add(new dish_allergen()
                        {
                            dish = dishToCommit,
                            allergen = (from a in _context.allergens
                                        where a.name == allergenName
                                        select a).First()
                        });
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Invalid allergen names");
                    }
                }
            }

            // Commit changes
            if (dishForm.Editing)
            {
                // empty
            }
            else
            {
                _context.products.Add(productToCommit);
                _context.dishes.Add(dishToCommit);
            }

            // Save changes to DB
            _context.SaveChanges();
        }

        public void CommitMenuFromForm(CrudFormsViewModel.MenuForm menuForm, List<menu_dish> menuDishes)
        {
            // Editing vs Adding
            product productToCommit;
            menu menuToCommit;
            if (menuForm.Editing)
            {
                menuToCommit = (from m in _context.menus
                                where m.id == menuForm.Id
                                select m).First();
                productToCommit = menuToCommit.product;
            }
            else
            {
                productToCommit = new product();
                menuToCommit = new menu();
            }

            // Change properties
            productToCommit.name = menuForm.Name;
            productToCommit.category = menuForm.Category;
            productToCommit.photos_source = menuForm.Name;
            productToCommit.photos_count = menuForm.PhotosCount;
            productToCommit.is_menu = true;

            menuToCommit.product = productToCommit;

            // Find old md
            List<int> menuDishesToRemoveIds = new List<int>();
            foreach (var menuDish in menuToCommit.menu_dish)
            {
                if (!menuDishes.Contains(menuDish))
                    menuDishesToRemoveIds.Add(menuDish.id);
            }

            // Delete old md
            foreach (var id in menuDishesToRemoveIds)
            {
                var menuDish = (from md in _context.menu_dish
                                where md.id == id
                                select md).First();
                _context.menu_dish.Remove(menuDish);
            }

            // Add / Update md
            foreach (var menuDish in menuDishes)
            {
                if (menuToCommit.menu_dish.Contains(menuDish))
                {
                    var existingMenuDish = (from md in menuToCommit.menu_dish
                                            where md.id == menuDish.id
                                            select md).First();
                    existingMenuDish.dish = menuDish.dish;
                    existingMenuDish.quantity = menuDish.quantity;
                }
                else
                {
                    menuToCommit.menu_dish.Add(new menu_dish()
                    {
                        menu = menuToCommit,
                        dish = menuDish.dish,
                        quantity = menuDish.quantity
                    });
                }
            }

            // Commit changes
            if (menuForm.Editing)
            {
                // empty
            }
            else
            {
                _context.products.Add(productToCommit);
                _context.menus.Add(menuToCommit);
            }

            // Save changes to DB
            _context.SaveChanges();
        }

        public void CommitCategories(List<category> categoriesList, List<category> categoriesToDelete)
        {
            // Add or Edit
            foreach (var categoryItem in categoriesList)
            {
                var result = (from c in _context.categories
                              where c.code == categoryItem.code
                              select c);

                if (result.Count() != 0)
                {
                    // Edit category name
                    result.First().name = categoryItem.name;
                }
                else
                {
                    // Add category
                    try
                    {
                        _context.categories.Add(categoryItem);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException($"Can't add category: {categoryItem.name}, category code already exists or it is too long (max 5 characters)");
                    }
                }
            }

            // Delete
            foreach (var categoryItem in categoriesToDelete)
            {
                _context.categories.Remove(categoryItem);
            }

            _context.SaveChanges();
        }

        public void CommitAllergens(List<allergen> allergensList, List<allergen> allergensToDelete)
        {
            // Add or Edit
            foreach (var allergenItem in allergensList)
            {
                var result = (from a in _context.allergens
                              where a.code == allergenItem.code
                              select a);

                if (result.Count() != 0)
                {
                    // Edit allergen name
                    result.First().code = allergenItem.code;
                    result.First().name = allergenItem.name;
                }
                else
                {
                    // Add allergen
                    try
                    {
                        _context.allergens.Add(allergenItem);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException($"Can't add allergen: {allergenItem.name}, allergen code already exists or it is too long (max 5 characters)");
                    }
                }
            }

            // Delete
            foreach (var allergenItem in allergensToDelete)
            {
                _context.allergens.Remove(allergenItem);
            }

            _context.SaveChanges();
        }

        public void DeleteDish(dish dishToDelete)
        {
            _context.products.Remove(dishToDelete.product);
            _context.dishes.Remove(dishToDelete);
            dishToDelete.menu_dish.Clear();

            _context.SaveChanges();
        }

        public void DeleteMenu(menu menuToDelete)
        {
            _context.products.Remove(menuToDelete.product);
            _context.menus.Remove(menuToDelete);

            _context.SaveChanges();
        }
    }
}
