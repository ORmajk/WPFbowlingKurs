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
    public partial class AddEditBookingPage : Page
    {
        private Bookings currentBooking = new Bookings();

        public AddEditBookingPage(Bookings selectedBooking)
        {
            InitializeComponent();

            cbClient.ItemsSource = AppConnect.model01.Clients.OrderBy(x => x.FullName).ToList();
            cbLane.ItemsSource = AppConnect.model01.Lanes.OrderBy(x => x.LaneNumber).ToList();
            cbPayment.ItemsSource = new List<string> { "Наличные", "Карта", "Онлайн" };
            cbStatus.ItemsSource = new List<string> { "Создан", "Оплачен", "Завершён", "Отменён" };

            List<int> durations = new List<int> { 30, 60, 90, 120, 180 };

            if (selectedBooking != null)
            {
                currentBooking = selectedBooking;
                tbTitle.Text = "Бронирование " + currentBooking.BookingNumber;
                dpDate.SelectedDate = currentBooking.StartTime.Date;
                tbTime.Text = currentBooking.StartTime.ToString("HH:mm");
                if (!durations.Contains(currentBooking.DurationMinutes))
                    durations.Add(currentBooking.DurationMinutes);
            }
            else
            {
                currentBooking.Status = "Создан";
                currentBooking.PaymentMethod = "Наличные";
                currentBooking.DurationMinutes = 60;
                dpDate.SelectedDate = DateTime.Today;
                tbTime.Text = "18:00";
            }

            cbDuration.ItemsSource = durations;
            cbDuration.SelectedItem = currentBooking.DurationMinutes;

            DataContext = currentBooking;
            CalcTotal();
        }

        private decimal CalcTotal()
        {
            decimal total = 0;
            var lane = cbLane.SelectedItem as Lanes;
            if (lane != null && cbDuration.SelectedItem != null)
            {
                int minutes = (int)cbDuration.SelectedItem;
                total = lane.PricePerHour * minutes / 60;
                total += currentBooking.BookingItems.Sum(x => x.Total);
                if (total < 0) total = 0;
            }
            tbTotal.Text = total.ToString("N2") + " руб.";
            return total;
        }

        private void cbLane_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbTotal != null) CalcTotal();
        }

        private void cbDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbTotal != null) CalcTotal();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (currentBooking.Clients == null)
                errors.AppendLine("Выберите клиента");
            if (currentBooking.Lanes == null)
                errors.AppendLine("Выберите дорожку");
            if (dpDate.SelectedDate == null)
                errors.AppendLine("Выберите дату");
            if (cbDuration.SelectedItem == null)
                errors.AppendLine("Выберите длительность");

            TimeSpan time;
            if (!TimeSpan.TryParse(tbTime.Text, out time) || time < TimeSpan.Zero || time.TotalHours >= 24)
                errors.AppendLine("Время введите в формате чч:мм, например 18:30");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime start = dpDate.SelectedDate.Value.Date + time;
            int minutes = (int)cbDuration.SelectedItem;
            DateTime end = start.AddMinutes(minutes);

            if (currentBooking.Id == 0 && start < DateTime.Now)
            {
                MessageBox.Show("Нельзя создать бронь на прошедшее время!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentBooking.Lanes.LaneStatuses.Name == "Требует обслуживания" && currentBooking.Status != "Отменён")
            {
                MessageBox.Show("Эта дорожка на обслуживании, выберите другую!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentBooking.Status != "Отменён")
            {
                int laneId = currentBooking.Lanes.Id;
                int bookingId = currentBooking.Id;
                var otherBookings = AppConnect.model01.Bookings
                    .Where(x => x.LaneId == laneId && x.Id != bookingId && x.Status != "Отменён")
                    .ToList();

                foreach (var b in otherBookings)
                {
                    DateTime bEnd = b.StartTime.AddMinutes(b.DurationMinutes);
                    if (start < bEnd && end > b.StartTime)
                    {
                        MessageBox.Show("Дорожка занята в это время! Бронь " + b.BookingNumber + " с " +
                            b.StartTime.ToString("HH:mm") + " до " + bEnd.ToString("HH:mm"), "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            currentBooking.StartTime = start;
            currentBooking.DurationMinutes = minutes;
            currentBooking.TotalAmount = CalcTotal();

            bool isNew = currentBooking.Id == 0;
            string awards = "";
            if (isNew)
            {
                int count = AppConnect.model01.Bookings.Count() + 1;
                string number = "BR-" + count.ToString("000");
                while (AppConnect.model01.Bookings.Any(x => x.BookingNumber == number))
                {
                    count++;
                    number = "BR-" + count.ToString("000");
                }
                currentBooking.BookingNumber = number;

                awards = PointsHelper.AddClientPoints(currentBooking.Clients, PointsHelper.PointsForBooking);

                AppConnect.model01.Bookings.Add(currentBooking);
            }

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(isNew ? "Создание бронирования" : "Редактирование",
                    "Бронирование " + currentBooking.BookingNumber + ", дорожка №" + currentBooking.Lanes.LaneNumber);
                MessageBox.Show("Бронирование сохранено! Номер: " + currentBooking.BookingNumber + "\n" + awards, "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + AppConnect.GetError(ex), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
                if (isNew)
                    currentBooking.BookingNumber = null;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
