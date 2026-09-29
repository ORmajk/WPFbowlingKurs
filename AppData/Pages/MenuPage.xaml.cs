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
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();

            if (AppConnect.currentUser.Role != "Admin")
            {
                btnUsers.Visibility = Visibility.Collapsed;
                btnJournal.Visibility = Visibility.Collapsed;
            }
            if (AppConnect.currentUser.Role == "Operator")
            {
                btnAwards.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadStatistic()
        {
            var lanes = AppConnect.model01.Lanes.ToList();
            tbAllLanes.Text = "Всего дорожек: " + lanes.Count;
            tbFreeLanes.Text = "Свободно: " + lanes.Count(x => x.LaneStatuses.Name == "Доступна");
            tbBusyLanes.Text = "Занято: " + lanes.Count(x => x.LaneStatuses.Name == "Занята");
            tbRepairLanes.Text = "На обслуживании: " + lanes.Count(x => x.LaneStatuses.Name == "Требует обслуживания");

            var bookings = AppConnect.model01.Bookings.ToList();
            tbTodayBookings.Text = "Броней на сегодня: " + bookings.Count(x => x.StartTime.Date == DateTime.Today && x.Status != "Отменён");

            decimal money = bookings.Where(x => x.Status != "Отменён").Sum(x => x.TotalAmount);
            tbMoney.Text = "Выручка: " + money.ToString("N2") + " руб.";
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                LoadStatistic();
                tbUser.Text = AppConnect.currentUser.FullName + " (" + AppConnect.GetRoleName(AppConnect.currentUser.Role) +
                    "), баллов: " + AppConnect.currentUser.Points;
            }
        }

        private void btnLanes_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new LanesPage());
        }

        private void btnBookings_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new BookingsPage());
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ClientsPage());
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new InventoryPage());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new ProductsPage());
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new UsersPage());
        }

        private void btnEvents_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new EventsPage());
        }

        private void btnAwards_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AwardsPage());
        }

        private void btnAi_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AiPage());
        }

        private void btnMyAwards_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new MyAwardsPage());
        }

        private void btnJournal_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new JournalPage());
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
