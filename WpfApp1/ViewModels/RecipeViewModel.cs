using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Ingredient> _ingredients;
        private ObservableCollection<string> _instructions;
        private ObservableCollection<Recipe> _recipes;
        private ObservableCollection<Recipe> _menu;
        private Recipe _selectedRecipe;
        private ObservableCollection<Recipe> _filteredRecipes;
        private ObservableCollection<string> _foodGroups;
        private string _selectedFoodGroup;
        private string _selectedFilterFoodGroup;
        private string _newInstruction;
        private string _newIngredientName;
        private double _newIngredientQuantity;
        private string _newIngredientUnit;
        private int _newIngredientCalories;
        private bool _isFilterApplied;
        private int _totalMenuCalories;

        public RecipeViewModel()
        {
            Ingredients = new ObservableCollection<Ingredient>();
            Instructions = new ObservableCollection<string>();
            Recipes = new ObservableCollection<Recipe>();
            Menu = new ObservableCollection<Recipe>();
            FilteredRecipes = new ObservableCollection<Recipe>(Recipes);
            FoodGroups = new ObservableCollection<string>
            {
                "Vegetables", "Fruits", "Grains", "Protein Foods", "Dairy", "Oils & Solid Fats", "Added Sugars", "Beverages"
            };

            AddInstructionCommand = new RelayCommand(AddInstruction);
            AddIngredientCommand = new RelayCommand(AddIngredient);
            AddRecipeCommand = new RelayCommand(AddRecipe);
            AddToMenuCommand = new RelayCommand(AddToMenu);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            RemoveFilterCommand = new RelayCommand(RemoveFilter);
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Ingredient> Ingredients
        {
            get => _ingredients;
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Instructions
        {
            get => _instructions;
            set
            {
                _instructions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Recipe> Recipes
        {
            get => _recipes;
            set
            {
                _recipes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Recipe> Menu
        {
            get => _menu;
            set
            {
                _menu = value;
                OnPropertyChanged();
            }
        }

        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged();
                if (_selectedRecipe != null)
                {
                    Name = _selectedRecipe.Name;
                    Ingredients = new ObservableCollection<Ingredient>(_selectedRecipe.Ingredients);
                    Instructions = new ObservableCollection<string>(_selectedRecipe.Instructions);
                }
            }
        }

        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set
            {
                _filteredRecipes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> FoodGroups
        {
            get => _foodGroups;
            set
            {
                _foodGroups = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFoodGroup
        {
            get => _selectedFoodGroup;
            set
            {
                _selectedFoodGroup = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFilterFoodGroup
        {
            get => _selectedFilterFoodGroup;
            set
            {
                _selectedFilterFoodGroup = value;
                OnPropertyChanged();
            }
        }

        public string NewInstruction
        {
            get => _newInstruction;
            set
            {
                _newInstruction = value;
                OnPropertyChanged();
            }
        }

        public string NewIngredientName
        {
            get => _newIngredientName;
            set
            {
                _newIngredientName = value;
                OnPropertyChanged();
            }
        }

        public double NewIngredientQuantity
        {
            get => _newIngredientQuantity;
            set
            {
                _newIngredientQuantity = value;
                OnPropertyChanged();
            }
        }

        public string NewIngredientUnit
        {
            get => _newIngredientUnit;
            set
            {
                _newIngredientUnit = value;
                OnPropertyChanged();
            }
        }

        public int NewIngredientCalories
        {
            get => _newIngredientCalories;
            set
            {
                _newIngredientCalories = value;
                OnPropertyChanged();
            }
        }

        public bool IsFilterApplied
        {
            get => _isFilterApplied;
            set
            {
                _isFilterApplied = value;
                OnPropertyChanged();
            }
        }

        public int TotalMenuCalories
        {
            get => _totalMenuCalories;
            set
            {
                _totalMenuCalories = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddInstructionCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand AddRecipeCommand { get; }
        public ICommand AddToMenuCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand RemoveFilterCommand { get; }

        private void AddInstruction()
        {
            if (!string.IsNullOrEmpty(NewInstruction) && SelectedRecipe != null)
            {
                SelectedRecipe.Instructions.Add(NewInstruction);
                NewInstruction = string.Empty;
            }
        }

        private void AddIngredient()
        {
            if (!string.IsNullOrEmpty(NewIngredientName) && NewIngredientQuantity > 0 && SelectedRecipe != null)
            {
                SelectedRecipe.Ingredients.Add(new Ingredient
                {
                    Name = NewIngredientName,
                    Quantity = NewIngredientQuantity,
                    Unit = NewIngredientUnit,
                    Calories = NewIngredientCalories,
                    FoodGroup = SelectedFoodGroup
                });

                NewIngredientName = string.Empty;
                NewIngredientQuantity = 0;
                NewIngredientUnit = string.Empty;
                NewIngredientCalories = 0;
            }
        }

        private void AddRecipe()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var newRecipe = new Recipe
                {
                    Name = Name,
                    Ingredients = new List<Ingredient>(Ingredients),
                    Instructions = new List<string>(Instructions)
                };

                Recipes.Add(newRecipe);

                Name = string.Empty;
                Ingredients.Clear();
                Instructions.Clear();
            }
        }

        private void AddToMenu()
        {
            if (SelectedRecipe != null && !Menu.Contains(SelectedRecipe))
            {
                Menu.Add(SelectedRecipe);
                UpdateTotalMenuCalories();
            }
        }

        private void ApplyFilter()
        {
            if (!string.IsNullOrEmpty(SelectedFilterFoodGroup))
            {
                FilteredRecipes.Clear();
                var filtered = Recipes.Where(r => r.Ingredients.Any(i => i.FoodGroup == SelectedFilterFoodGroup));
                foreach (var recipe in filtered)
                {
                    FilteredRecipes.Add(recipe);
                }
                IsFilterApplied = true;
            }
        }

        private void RemoveFilter()
        {
            FilteredRecipes.Clear();
            foreach (var recipe in Recipes)
            {
                FilteredRecipes.Add(recipe);
            }
            IsFilterApplied = false;
        }

        private void UpdateTotalMenuCalories()
        {
            TotalMenuCalories = Menu.Sum(r => r.Ingredients.Sum(i => i.Calories));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
