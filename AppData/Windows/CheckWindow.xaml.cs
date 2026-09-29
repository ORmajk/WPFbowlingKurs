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
using System.Windows.Shapes;

namespace WPFbowlingKurs.AppData.Windows
{
    public partial class CheckWindow : Window
    {
        private Bookings booking;

        public CheckWindow(Bookings selectedBooking)
        {
            InitializeComponent();
            booking = selectedBooking;

            tbNumber.Text = "Чек по брони: " + booking.BookingNumber;
            tbDate.Text = "Дата печати: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            tbClient.Text = "Клиент: " + booking.Clients.FullName + ", " + booking.Clients.Phone;
            tbLane.Text = "Дорожка №" + booking.Lanes.LaneNumber + " (" + booking.Lanes.LaneTypes.Name + "), " +
                booking.Lanes.PricePerHour.ToString("N0") + " руб./час";
            tbTime.Text = "Время: " + booking.StartTime.ToString("dd.MM.yyyy HH:mm") + ", " + booking.DurationMinutes + " мин.";

            StringBuilder items = new StringBuilder();
            foreach (var item in booking.BookingItems)
                items.AppendLine(item.ItemName + ": " + item.Total.ToString("N2") + " руб.");
            tbItems.Text = items.ToString().Trim();
            if (tbItems.Text == "")
                tbItems.Visibility = Visibility.Collapsed;

            tbPayment.Text = "Способ оплаты: " + booking.PaymentMethod;
            tbStatus.Text = "Статус: " + booking.Status;
            tbTotal.Text = "ИТОГО: " + booking.TotalAmount.ToString("N2") + " руб.";
            tbCashier.Text = "Чек сформировал: " + AppConnect.currentUser.FullName;

            try
            {
                imgQr.Source = QrHelper.GetQrImage(QrHelper.GetPaymentText(booking));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось создать QR-код: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(brdCheck, "Чек " + booking.BookingNumber);
                AppConnect.AddLog("Печать чека", "Напечатан чек по брони " + booking.BookingNumber);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
