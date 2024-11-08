using PetShop_petro.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PetShop_petro.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewProductsPage.xaml
    /// </summary>
    public partial class ViewProductsPage : Page
    {
        public object Classes { get; private set; }
        public object CountOfLabel { get; private set; }

        public ViewProductsPage()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            ProductListView.ItemsSource = PetModel.PetrouEntities.GetContext().Product.ToList();
            var manufactList = PetModel.PetrouEntities.GetContext().Manufacrture.ToList();
            manufactList.Insert(0, new PetModel.Manufacrture { manufac = "Все производители" });
            ManufacturerComboBox.ItemsSource = manufactList;
            ManufacturerComboBox.SelectedIndex = 0;

            if (Manager.CurrentUser != null)
            {
                FIOLable.Visibility = Visibility.Visible;
                FIOLable.Content = $"{Manager.CurrentUser.UserSurname} {Manager.CurrentUser.UserName} {Manager.CurrentUser.UserPatronymic}";
            }
            else
            {
                FIOLable.Visibility = Visibility.Hidden;
            }

            CountOfLable.Content = $"{PetModel.PetrouEntities.GetContext().Product.Count()}/{PetModel.PetrouEntities.GetContext().Product.Count()}";
        }

        public List<PetModel.Product> _currentProduct = PetModel.PetrouEntities.GetContext().Product.ToList();

        public void Update()
        {
            try
            {
                _currentProduct = PetModel.PetrouEntities.GetContext().Product.ToList();
                _currentProduct = (from item in _currentProduct
                                   where item.ProductName.name.ToLower().Contains(SearchTexBox.Text.ToLower()) ||
                                   item.Description.ToLower().Contains(SearchTexBox.Text.ToLower()) ||
                                   item.Manufacrture.manufac.ToLower().Contains(SearchTexBox.Text.ToLower()) ||
                                   item.ProductCost.ToString().ToLower().Contains(SearchTexBox.Text.ToLower()) ||
                                   item.QuantityInStock.ToString().ToLower().Contains(SearchTexBox.Text.ToLower())
                                   select item).ToList();

                if (SortUpRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderBy(d => d.ProductCost).ToList();
                }
                if (SortDownRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderByDescending(d => d.ProductCost).ToList();
                }

                var selected = ManufacturerComboBox.SelectedItem as PetModel.Manufacrture;
                if (selected != null && selected.manufac != "Все производители")
                {
                    _currentProduct = _currentProduct.Where(d => d.Manufacrture.id == selected.id).ToList();
                }

                CountOfLable.Content = $"{_currentProduct.Count}/{PetModel.PetrouEntities.GetContext().Product.Count()}";

                ProductListView.ItemsSource = _currentProduct;
            }
            catch (Exception)
            {

            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
            {
                if (Manager.CurrentUser != null)
                {
                    Manager.CurrentUser = null;
                }

                Manager.MainFrame.GoBack();
            }
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
    }
}
