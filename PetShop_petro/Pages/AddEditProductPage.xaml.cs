using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PetShop_petro.Pages
{
    /// <summary>
    /// Interaction logic for AddEditProductPage.xaml
    /// </summary>
    public partial class AddEditProductPage : Page
    {
        public PetModel.Product NewUser { get; set; }
        public string FlagAddOrEdit = "default";
        public PetModel.Product _currentProduct = new PetModel.Product();
        public AddEditProductPage(PetModel.Product product)

        {
            InitializeComponent();

            if (product != null)
            {

                _currentProduct = product;
                FlagAddOrEdit = "edit";
            }
            else 
            {
                FlagAddOrEdit = "add";
            }
            DataContext = _currentProduct;

            Init();
        }
        public void Init()
        {
            NewUser = new PetModel.Product();
            try
            {
                var pvp = PetModel.PetrouEntities.GetContext().Category.ToList();
                CategoryComboBox.ItemsSource = pvp;

                if (FlagAddOrEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Hidden;
                    IdLabel.Visibility = Visibility.Hidden;
                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = String.Empty;  
                    UnitTextBox.Text = String.Empty;    
                    NameTextBox.Text = String.Empty;    
                    CostTextBox.Text = String.Empty;    
                    SupplierTextBox.Text = String.Empty;    
                    DescriptionTextBox.Text = String.Empty;
                }
                else if (FlagAddOrEdit == "edit")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;

                    CategoryComboBox.SelectedItem = _currentProduct;
                    CountTextBox.Text = _currentProduct.QuantityInStock.ToString();
                    UnitTextBox.Text = _currentProduct.Units.NameOfUnit;//Надо создать новую табличку и обновить данные (Урок 6 12 минута)    
                    NameTextBox.Text = _currentProduct.ProductName.name;//Вместо "name" навзание столбца в СУБД
                    CostTextBox.Text = _currentProduct.ProductCost.ToString();
                    SupplierTextBox.Text = _currentProduct.Manufacrture.manufac;//название поставщика из СУБД
                    DescriptionTextBox.Text = _currentProduct.Description;

                    IdTextBox.Text = PetModel.PetrouEntities.GetContext().Product.Max(d => d.id+1).ToString();
                    CategoryComboBox.SelectedItem = PetModel.PetrouEntities.GetContext().Category.Where(d => d.id == _currentProduct.id).FirstOrDefault();
                }
                     
            }
            catch (Exception)
            {

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.AdminLKPage());
            return;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (CategoryComboBox.SelectedItem == null )
                {
                    errors.AppendLine("Выбирите категорию");
                }
                if (string.IsNullOrEmpty(CountTextBox.Text))
                {
                    errors.AppendLine("Заполинте количество");
                }
                else 
                {
                    var countTry = Int32.TryParse(CountTextBox.Text, out var resultCount);
                    if (!countTry)
                    {
                        errors.AppendLine("Количество целое число");
                    }
                    else 
                    {
                        if ((resultCount < 0))
                        {
                            errors.AppendLine("Количество не может быть отрицательным");
                        }
                    }

                }
                if (string.IsNullOrEmpty(UnitTextBox.Text))
                {
                    errors.AppendLine("Заполинте ед. изм.");
                }
                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполинте наименование");
                }
                if (string.IsNullOrEmpty(CostTextBox.Text))
                {
                    errors.AppendLine("Заполинте стоимость");
                }
                else
                {
                    var costTry = Decimal.TryParse(CostTextBox.Text, out var resultCost);
                    if (!costTry)
                    {
                        errors.AppendLine("Стоимость дробное число");
                    }
                    else
                    {
                        if (resultCost < 0)
                        {
                            errors.AppendLine("Стоимость не может быть отрицательной!");
                        }
                        else 
                        {
                            //Проверка на 2 знака после запятой
                        }

                    }
                }
                if (string.IsNullOrEmpty(SupplierTextBox.Text))
                {
                    errors.AppendLine("Заполинте постащика");
                }
                if (string.IsNullOrEmpty(DescriptionTextBox.Text))
                {
                    errors.AppendLine("Заполинте описание");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedCategory = CategoryComboBox.SelectedItem as PetModel.Category;

                _currentProduct.id = PetModel.PetrouEntities.GetContext().Category.Where(d => d.id == selectedCategory.id).FirstOrDefault().id;
                _currentProduct.QuantityInStock = Convert.ToInt32(CountTextBox.Text);
                _currentProduct.ProductCost = Convert.ToDecimal(CostTextBox.Text);
                _currentProduct.Description = DescriptionTextBox.Text;


                var searchUnit = (from item in PetModel.PetrouEntities.GetContext().Units
                                  where item.NameOfUnit == UnitTextBox.Text
                                  select item).FirstOrDefault();
                if (searchUnit != null)
                {
                    _currentProduct.IdUnit = searchUnit.id;
                }
                else
                {
                    PetModel.Units units = new PetModel.Units()
                    {
                        NameOfUnit = UnitTextBox.Text
                    };
                    PetModel.PetrouEntities.GetContext().Units.Add(units);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    _currentProduct.IdUnit = units.id;
                }

                if (FlagAddOrEdit == "add")
                {
                    PetModel.PetrouEntities.GetContext().Product.Add(_currentProduct);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добвалено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddOrEdit == "edit")
                {
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно сохранено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var searchSupplier = (from man in PetModel.PetrouEntities.GetContext().Manufacrture
                                  where man.manufac == SupplierTextBox.Text
                                  select man).FirstOrDefault();
                if (searchSupplier != null)
                {
                    _currentProduct.id = searchSupplier.id;
                }
                else
                {
                    PetModel.Manufacrture suppliers = new PetModel.Manufacrture()
                    {
                        manufac = SupplierTextBox.Text
                    };
                    PetModel.PetrouEntities.GetContext().Manufacrture.Add(suppliers);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    _currentProduct.id = suppliers.id;

                }
                if (FlagAddOrEdit == "add")
                {
                    PetModel.PetrouEntities.GetContext().Product.Add(_currentProduct);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добвалено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddOrEdit == "edit")
                {
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно сохранено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }




                var searchName = (from nam in PetModel.PetrouEntities.GetContext().ProductName
                                      where nam.name == NameTextBox.Text
                                      select nam).FirstOrDefault();
                if (searchName != null)
                {
                    _currentProduct.id = searchName.id;
                }
                else
                {
                    PetModel.ProductName names = new PetModel.ProductName()
                    {
                        name = NameTextBox.Text
                    };
                    PetModel.PetrouEntities.GetContext().ProductName.Add(names);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    _currentProduct.id = names.id;
                }
                if (FlagAddOrEdit == "add")
                {
                    PetModel.PetrouEntities.GetContext().Product.Add(_currentProduct);
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добвалено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddOrEdit == "edit")
                {
                    PetModel.PetrouEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно сохранено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Успех!", "Готово!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetImage();
        }
        private void SetImage()
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "Изображения (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg";
            string Path = "";
            if (Dialog.ShowDialog() == true)
            {
                Path = Dialog.FileName;
            }
            if (string.IsNullOrEmpty(Path))
            {
                return;
            }
            BitmapImage Image = new BitmapImage(new Uri(Path));
            ProductImage.Source = Image;
            NewUser.Image = File.ReadAllBytes(Path);
            NewUser.ImageName = Path.Split('\\').Last().ToString();
        }
    }
}
