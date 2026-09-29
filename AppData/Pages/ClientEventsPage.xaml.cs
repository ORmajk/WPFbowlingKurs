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
    public partial class ClientEventsPage : Page
    {
        private Clients client;

        public ClientEventsPage()
        {
            InitializeComponent();
        }

        private void UpdateEvents()
        {
            client = AppConnect.GetCurrentClient();
            var events = AppConnect.model01.Events.ToList();

            if (chbAll.IsChecked != true)
                events = events.Where(x => x.EventDate >= DateTime.Today).ToList();

            foreach (var ev in events)
            {
                if (ev.EventRegistrations.Any(x => x.ClientId == client.Id))
                    ev.MyStatus = "✔ Вы записаны";
                else
                    ev.MyStatus = "";
            }

            lvEvents.ItemsSource = events.OrderBy(x => x.EventDate).ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                UpdateEvents();
            }
        }

        private void chbAll_Changed(object sender, RoutedEventArgs e)
        {
            UpdateEvents();
        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvEvents.SelectedItem as Events;
            if (selected == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.EventDate < DateTime.Today)
            {
                MessageBox.Show("Это мероприятие уже прошло!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.EventRegistrations.Any(x => x.ClientId == client.Id))
            {
                MessageBox.Show("Вы уже записаны на это мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            EventRegistrations reg = new EventRegistrations();
            reg.Clients = client;
            reg.Events = selected;
            AppConnect.model01.EventRegistrations.Add(reg);

            string awards = PointsHelper.AddClientPoints(client, PointsHelper.PointsForEvent);
            if (PointsHelper.GiveAward(AppConnect.currentUser, "Участник мероприятий", "Запись на мероприятие клуба"))
                awards += "Получена награда: «Участник мероприятий»";

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("Мероприятие", "Клиент записался на " + selected.Name);
                MessageBox.Show("Вы записаны на «" + selected.Name + "»! +" + PointsHelper.PointsForEvent + " баллов\n" + awards,
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
                UpdateEvents();
            }
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvEvents.SelectedItem as Events;
            if (selected == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var reg = selected.EventRegistrations.FirstOrDefault(x => x.ClientId == client.Id);
            if (reg == null)
            {
                MessageBox.Show("Вы не записаны на это мероприятие.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Отменить запись на «" + selected.Name + "»?\nНачисленные баллы будут списаны.", "Отмена записи",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    AppConnect.model01.EventRegistrations.Remove(reg);
                    client.Points = Math.Max(0, client.Points - PointsHelper.PointsForEvent);
                    AppConnect.currentUser.Points = Math.Max(0, AppConnect.currentUser.Points - PointsHelper.PointsForEvent);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Мероприятие", "Клиент отменил запись на " + selected.Name);
                    UpdateEvents();
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
            AppFrame.frameMain.GoBack();
        }
    }
}
