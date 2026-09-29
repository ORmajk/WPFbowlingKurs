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

namespace WPFbowlingKurs.AppData.Pages
{
    public partial class ProductsPage : Page
    {
        private List<ProductCard> allProducts = new List<ProductCard>();

        public ProductsPage()
        {
            InitializeComponent();

            try
            {
                allProducts = AppConnect.model01.Database.SqlQuery<ProductCard>(
                    "SELECT Id, Name, Category, Description, Price, Quantity, ImageUrl, IsVisible FROM Products").ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить товары. Проверьте, что выполнен скрипт BowlingClub_site.sql.\n" +
                    AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            List<string> categories = new List<string> { "Все категории" };
            categories.AddRange(allProducts.Select(x => x.Category).Distinct().OrderBy(x => x));
            cbCategory.ItemsSource = categories;
            cbCategory.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
        }

        private void UpdateProducts()
        {
            var products = allProducts.ToList();

            if (tbSearch.Text != "")
                products = products.Where(x => x.Name.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

            if (cbCategory.SelectedIndex > 0)
                products = products.Where(x => x.Category == cbCategory.SelectedItem.ToString()).ToList();

            switch (cbSort.SelectedIndex)
            {
                case 1:
                    products = products.OrderBy(x => x.Price).ToList();
                    break;
                case 2:
                    products = products.OrderByDescending(x => x.Price).ToList();
                    break;
                case 3:
                    products = products.OrderBy(x => x.Quantity).ToList();
                    break;
                default:
                    products = products.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
                    break;
            }

            lvProducts.ItemsSource = products;
            tbCount.Text = "Найдено: " + products.Count;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
