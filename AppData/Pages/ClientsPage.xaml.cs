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
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();

            if (AppConnect.currentUser.Role != "Admin")
                btnDelete.Visibility = Visibility.Collapsed;
        }

        private void UpdateClients()
        {
            var clients = AppConnect.model01.Clients.ToList();

            if (tbSearch.Text != "")
            {
                string search = tbSearch.Text.ToLower();
                clients = clients.Where(x => x.FullName.ToLower().Contains(search)
                    || (x.Phone != null && x.Phone.Contains(search))
                    || (x.Email != null && x.Email.ToLower().Contains(search))).ToList();
            }

            clients = clients.OrderBy(x => x.FullName).ToList();
            dgClients.ItemsSource = clients;
            tbCount.Text = "Найдено: " + clients.Count;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateClients();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditClientPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgClients.SelectedItem as Clients;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditClientPage(selected));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgClients.SelectedItem as Clients;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Bookings.Count > 0)
            {
                MessageBox.Show("Нельзя удалить клиента, у него есть бронирования!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Удалить клиента " + selected.FullName + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string name = selected.FullName;
                    AppConnect.model01.EventRegistrations.RemoveRange(selected.EventRegistrations.ToList());
                    AppConnect.model01.Clients.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалён клиент " + name);
                    AppConnect.RefreshData();
                    UpdateClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + AppConnect.GetError(ex), "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
