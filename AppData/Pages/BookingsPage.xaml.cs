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
    public partial class BookingsPage : Page
    {
        public BookingsPage()
        {
            InitializeComponent();

            cbStatus.ItemsSource = new List<string> { "Все", "Создан", "Оплачен", "Завершён", "Отменён" };
            cbStatus.SelectedIndex = 0;

            if (AppConnect.currentUser.Role != "Admin")
                btnDelete.Visibility = Visibility.Collapsed;
        }

        private void UpdateBookings()
        {
            var bookings = AppConnect.model01.Bookings.ToList();

            if (tbSearch.Text != "")
            {
                string search = tbSearch.Text.ToLower();
                bookings = bookings.Where(x => x.BookingNumber.ToLower().Contains(search)
                    || x.Clients.FullName.ToLower().Contains(search)).ToList();
            }

            if (cbStatus.SelectedIndex > 0)
                bookings = bookings.Where(x => x.Status == cbStatus.SelectedItem.ToString()).ToList();

            if (dpDate.SelectedDate != null)
                bookings = bookings.Where(x => x.StartTime.Date == dpDate.SelectedDate.Value.Date).ToList();

            bookings = bookings.OrderByDescending(x => x.StartTime).ToList();
            dgBookings.ItemsSource = bookings;
            tbCount.Text = "Найдено: " + bookings.Count;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateBookings();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBookings();
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBookings();
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBookings();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            cbStatus.SelectedIndex = 0;
            dpDate.SelectedDate = null;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditBookingPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронирование!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditBookingPage(selected));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронирование!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.Status == "Отменён" || selected.Status == "Завершён")
            {
                MessageBox.Show("Эту бронь нельзя отменить, её статус: " + selected.Status, "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Отменить бронь " + selected.BookingNumber + "?", "Отмена брони",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    selected.Status = "Отменён";
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Отмена", "Отменено бронирование " + selected.BookingNumber);
                    UpdateBookings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронирование!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Удалить бронь " + selected.BookingNumber + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string number = selected.BookingNumber;
                    AppConnect.model01.BookingItems.RemoveRange(selected.BookingItems.ToList());
                    AppConnect.model01.Bookings.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалено бронирование " + number);
                    AppConnect.RefreshData();
                    UpdateBookings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + AppConnect.GetError(ex), "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBookings.SelectedItem as Bookings;
            if (selected == null)
            {
                MessageBox.Show("Выберите бронирование!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Windows.CheckWindow window = new Windows.CheckWindow(selected);
            window.ShowDialog();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var bookings = dgBookings.ItemsSource as List<Bookings>;
            if (bookings == null) return;

            var list = bookings.Select(x => new
            {
                Номер = x.BookingNumber,
                Клиент = x.Clients.FullName,
                Дорожка = x.Lanes.LaneNumber,
                Начало = x.StartTime,
                Минут = x.DurationMinutes,
                Сумма = x.TotalAmount,
                Оплата = x.PaymentMethod,
                Статус = x.Status
            }).ToList();

            CsvExporter.Export(list, "Бронирования.csv");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
