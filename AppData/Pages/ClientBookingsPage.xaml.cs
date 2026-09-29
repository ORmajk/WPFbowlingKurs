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
    public partial class ClientBookingsPage : Page
    {
        public ClientBookingsPage()
        {
            InitializeComponent();
        }

        private void UpdateBookings()
        {
            var client = AppConnect.GetCurrentClient();
            int clientId = client.Id;
            var bookings = AppConnect.model01.Bookings.Where(x => x.ClientId == clientId).ToList();

            if (chbOnlyActive.IsChecked == true)
                bookings = bookings.Where(x => x.StartTime.AddMinutes(x.DurationMinutes) >= DateTime.Now && x.Status != "Отменён").ToList();

            bookings = bookings.OrderByDescending(x => x.StartTime).ToList();
            dgBookings.ItemsSource = bookings;
            tbCount.Text = "Броней: " + bookings.Count;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                UpdateBookings();
            }
        }

        private void chbOnlyActive_Changed(object sender, RoutedEventArgs e)
        {
            if (dgBookings != null && AppConnect.currentUser != null)
                UpdateBookings();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientBookingPage(null));
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронь!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Windows.CheckWindow window = new Windows.CheckWindow(selected);
            window.ShowDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронь!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.Status != "Создан")
            {
                MessageBox.Show("Отменить можно только бронь со статусом «Создан». Статус этой брони: " + selected.Status +
                    ".\nПо оплаченной брони обратитесь к администратору.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.StartTime < DateTime.Now.AddHours(2))
            {
                MessageBox.Show("Отменить бронь можно не позже, чем за 2 часа до начала.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Отменить бронь " + selected.BookingNumber + "?\nБаллы за бронь будут списаны, а баллы, " +
                "потраченные на оплату, вернутся.", "Отмена брони", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    selected.Status = "Отменён";

                    int spent = selected.BookingItems.Where(x => x.ItemName == "Оплата баллами").Sum(x => x.Quantity);
                    int change = spent - PointsHelper.PointsForBooking;
                    AppConnect.currentUser.Points = Math.Max(0, AppConnect.currentUser.Points + change);
                    selected.Clients.Points = Math.Max(0, selected.Clients.Points + change);

                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Отмена", "Клиент отменил бронь " + selected.BookingNumber);
                    MessageBox.Show("Бронь отменена.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateBookings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientMenuPage());
        }
    }
}
