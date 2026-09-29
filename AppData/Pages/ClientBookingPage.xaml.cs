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
    public partial class ClientBookingPage : Page
    {
        private Clients client;

        public ClientBookingPage(Lanes selectedLane)
        {
            InitializeComponent();

            client = AppConnect.GetCurrentClient();

            var lanes = AppConnect.model01.Lanes.ToList()
                .Where(x => x.LaneStatuses.Name != "Требует обслуживания")
                .OrderBy(x => x.LaneNumber).ToList();
            cbLane.ItemsSource = lanes;

            List<string> times = new List<string>();
            for (int h = 10; h <= 23; h++)
            {
                times.Add(h.ToString("00") + ":00");
                if (h < 23) times.Add(h.ToString("00") + ":30");
            }
            cbTime.ItemsSource = times;
            cbTime.SelectedItem = "18:00";

            cbDuration.ItemsSource = new List<int> { 60, 90, 120, 180 };
            cbDuration.SelectedIndex = 0;
            cbPayment.ItemsSource = new List<string> { "Карта", "Наличные", "Онлайн" };
            cbPayment.SelectedIndex = 0;

            dpDate.SelectedDate = DateTime.Today;
            dpDate.DisplayDateStart = DateTime.Today;

            if (selectedLane != null)
                cbLane.SelectedItem = lanes.FirstOrDefault(x => x.Id == selectedLane.Id);
            else if (lanes.Count > 0)
                cbLane.SelectedIndex = 0;

            CalcTotal();
        }

        private decimal GetPrice()
        {
            var lane = cbLane.SelectedItem as Lanes;
            if (lane == null || cbDuration.SelectedItem == null)
                return 0;
            int minutes = (int)cbDuration.SelectedItem;
            return lane.PricePerHour * minutes / 60;
        }

        private int GetMaxPoints()
        {
            int max = (int)(GetPrice() * 30 / 100);
            return Math.Min(max, AppConnect.currentUser.Points);
        }

        private void CalcTotal()
        {
            if (tbTotal == null) return;

            decimal price = GetPrice();
            int points = GetMaxPoints();
            tbPointsText.Text = "Списать баллы: " + points + " (у вас " + AppConnect.currentUser.Points +
                " баллов, 1 балл = 1 руб., не больше 30% суммы)";
            chbPoints.IsEnabled = points > 0;
            if (points == 0) chbPoints.IsChecked = false;

            decimal total = price;
            if (chbPoints.IsChecked == true)
                total -= points;
            tbTotal.Text = "К оплате: " + total.ToString("N2") + " руб.";

            var lane = cbLane.SelectedItem as Lanes;
            if (lane != null)
            {
                try
                {
                    imgLane.Source = new BitmapImage(new Uri(lane.PhotoPath));
                }
                catch
                {
                    imgLane.Source = null;
                }
                tbLaneInfo.Text = "Дорожка №" + lane.LaneNumber + "\n" + lane.LaneTypes.Name + "\n" +
                    lane.LaneTypes.Description + "\nВместимость: " + lane.Capacity + " чел.";
            }
        }

        private void cbLane_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalcTotal();
        }

        private void cbDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalcTotal();
        }

        private void chbPoints_Changed(object sender, RoutedEventArgs e)
        {
            CalcTotal();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            var lane = cbLane.SelectedItem as Lanes;

            if (lane == null) errors.AppendLine("Выберите дорожку");
            if (dpDate.SelectedDate == null) errors.AppendLine("Выберите дату");
            if (cbTime.SelectedItem == null) errors.AppendLine("Выберите время");
            if (cbDuration.SelectedItem == null) errors.AppendLine("Выберите длительность");
            if (cbPayment.SelectedItem == null) errors.AppendLine("Выберите способ оплаты");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime start = dpDate.SelectedDate.Value.Date + TimeSpan.Parse(cbTime.SelectedItem.ToString());
            int minutes = (int)cbDuration.SelectedItem;
            DateTime end = start.AddMinutes(minutes);

            if (start < DateTime.Now)
            {
                MessageBox.Show("Нельзя забронировать на прошедшее время!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (end > dpDate.SelectedDate.Value.Date.AddDays(1))
            {
                MessageBox.Show("Клуб работает до 00:00, уменьшите длительность или выберите время раньше.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int laneId = lane.Id;
            var otherBookings = AppConnect.model01.Bookings
                .Where(x => x.LaneId == laneId && x.Status != "Отменён")
                .ToList();
            foreach (var b in otherBookings)
            {
                DateTime bEnd = b.StartTime.AddMinutes(b.DurationMinutes);
                if (start < bEnd && end > b.StartTime)
                {
                    MessageBox.Show("Дорожка занята с " + b.StartTime.ToString("HH:mm") + " до " + bEnd.ToString("HH:mm") +
                        ". Выберите другое время или дорожку.",
                        "Дорожка занята", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Bookings booking = new Bookings();
            booking.Clients = client;
            booking.Lanes = lane;
            booking.StartTime = start;
            booking.DurationMinutes = minutes;
            booking.PaymentMethod = cbPayment.SelectedItem.ToString();
            booking.Status = "Создан";
            booking.TotalAmount = GetPrice();

            int count = AppConnect.model01.Bookings.Count() + 1;
            string number = "BR-" + count.ToString("000");
            while (AppConnect.model01.Bookings.Any(x => x.BookingNumber == number))
            {
                count++;
                number = "BR-" + count.ToString("000");
            }
            booking.BookingNumber = number;

            int points = 0;
            if (chbPoints.IsChecked == true)
            {
                points = GetMaxPoints();
                if (points > 0 && PointsHelper.SpendPoints(AppConnect.currentUser, points))
                {
                    BookingItems item = new BookingItems();
                    item.Bookings = booking;
                    item.ItemName = "Оплата баллами";
                    item.Quantity = points;
                    item.Price = -1;
                    item.Total = -points;
                    booking.BookingItems.Add(item);
                    booking.TotalAmount -= points;
                }
            }

            AppConnect.model01.Bookings.Add(booking);
            string awards = PointsHelper.AddClientPoints(client, PointsHelper.PointsForBooking);

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("Бронирование", "Клиент забронировал дорожку №" + lane.LaneNumber + ", бронь " + number);
                MessageBox.Show("Дорожка забронирована! Номер брони: " + number + "\n+" + PointsHelper.PointsForBooking +
                    " баллов\n" + awards, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                Windows.CheckWindow window = new Windows.CheckWindow(booking);
                window.ShowDialog();

                AppFrame.frameMain.Navigate(new ClientBookingsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при бронировании: " + AppConnect.GetError(ex), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
