using PetShop_petro.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShop_petro.Pages
{
    /// <summary>
    /// Interaction logic for AdminLKPage.xaml
    /// </summary>
    public partial class AdminLKPage : Page
    {
        public object Classes { get; private set; }
        public object CountOfLabel { get; private set; }

        public AdminLKPage()
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

            Manager.MainFrame.Navigate(new Pages.AddEditProductPage((sender as Button).DataContext as PetModel.Product));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = (sender as Button).DataContext as PetModel.Product;
                var orderProduct = PetModel.PetrouEntities.GetContext().OrderProduct.Where(d => d.idProduct == selected.id).ToList();

                if (orderProduct.Count() > 0)
                {
                    MessageBox.Show("товар из заказа удалить нельзя!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else {

                    PetModel.PetrouEntities.GetContext().Product.Remove(selected);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно удалено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Update();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.Page1());
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.AddEditProductPage(null));
        }
    }
}
    