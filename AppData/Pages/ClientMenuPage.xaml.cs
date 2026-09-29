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
    public partial class ClientMenuPage : Page
    {
        public ClientMenuPage()
        {
            InitializeComponent();
        }

        private void LoadInfo()
        {
            var user = AppConnect.currentUser;
            tbUser.Text = user.FullName + " (Клиент)";
            tbPoints.Text = "Баллов: " + user.Points;
            tbAwards.Text = "Наград: " + user.UserAwards.Count;
            tbAi.Text = user.IsSubscribedToAI ? "AI-помощник: подключён" : "AI-помощник: не подключён";

            var client = AppConnect.GetCurrentClient();
            var next = client.Bookings
                .Where(x => x.StartTime >= DateTime.Now && x.Status != "Отменён")
                .OrderBy(x => x.StartTime)
                .FirstOrDefault();
            if (next != null)
                tbNextBooking.Text = "Ближайшая бронь: " + next.BookingNumber + ", " + next.StartTime.ToString("dd.MM.yyyy HH:mm") +
                    ", дорожка №" + next.Lanes.LaneNumber;
            else
                tbNextBooking.Text = "Ближайших броней нет";

            int events = client.EventRegistrations.Count(x => x.Events.EventDate >= DateTime.Today);
            tbEvents.Text = "Записей на мероприятия: " + events;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                LoadInfo();
            }
        }

        private void btnLanes_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new LanesPage());
        }

        private void btnNewBooking_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientBookingPage(null));
        }

        private void btnMyBookings_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientBookingsPage());
        }

        private void btnEvents_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientEventsPage());
        }

        private void btnAwards_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new MyAwardsPage());
        }

        private void btnAi_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AiPage());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ProductsPage());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppConnect.AddLog("Выход", "Выход из системы: " + AppConnect.currentUser.Login);
                AppConnect.currentUser = null;
                AppFrame.frameMain.Navigate(new Authorization());
            }
        }
    }
}
