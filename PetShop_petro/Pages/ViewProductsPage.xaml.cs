using PetShop_petro.Classes;
using PetShop_petro.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            ProductListView.ItemsSource = Data.PetrouEntities.GetContext().Product.ToList();
            var manufactList = Data.PetrouEntities.GetContext().Manufacrture.ToList();
            manufactList.Insert(0, new Manufacrture {manufac = "Все производители"});
            ManufacturerComboBox.ItemsSource = manufactList;
            ManufacturerComboBox.SelectedIndex = 0;
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void Update()
        {
            throw new NotImplementedException();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

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
