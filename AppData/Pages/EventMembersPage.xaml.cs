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
    public partial class EventMembersPage : Page
    {
        private Events currentEvent;

        public EventMembersPage(Events selectedEvent)
        {
            InitializeComponent();
            currentEvent = selectedEvent;

            tbTitle.Text = "Участники: " + currentEvent.Name;
            string lane = currentEvent.Lanes != null ? "дорожка №" + currentEvent.Lanes.LaneNumber : "все дорожки";
            tbInfo.Text = "Дата: " + currentEvent.EventDate.ToString("dd.MM.yyyy") + ", " + lane + ". " + currentEvent.Description;

            UpdateMembers();
        }

        private void UpdateMembers()
        {
            var members = currentEvent.EventRegistrations.OrderBy(x => x.Clients.FullName).ToList();
            dgMembers.ItemsSource = members;
            tbCount.Text = "Участников: " + members.Count;

            var ids = members.Select(x => x.ClientId).ToList();
            cbClient.ItemsSource = AppConnect.model01.Clients.Where(x => !ids.Contains(x.Id)).OrderBy(x => x.FullName).ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var client = cbClient.SelectedItem as Clients;
            if (client == null)
            {
                MessageBox.Show("Выберите клиента из списка!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (currentEvent.EventDate < DateTime.Today)
            {
                MessageBox.Show("Мероприятие уже прошло, записать нельзя!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EventRegistrations reg = new EventRegistrations();
            reg.Clients = client;
            reg.Events = currentEvent;
            AppConnect.model01.EventRegistrations.Add(reg);

            string awards = PointsHelper.AddClientPoints(client, PointsHelper.PointsForEvent);
            var user = AppConnect.FindClientUserByPhone(client.Phone);
            if (user != null && PointsHelper.GiveAward(user, "Участник мероприятий", "Запись на мероприятие клуба"))
                awards += "Получена награда: «Участник мероприятий»";

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("Мероприятие", "Клиент " + client.FullName + " записан на " + currentEvent.Name);
                MessageBox.Show("Клиент записан! +" + PointsHelper.PointsForEvent + " баллов\n" + awards, "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
                UpdateMembers();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgMembers.SelectedItem as EventRegistrations;
            if (selected == null)
            {
                MessageBox.Show("Выберите участника!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Убрать " + selected.Clients.FullName + " из списка участников?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string name = selected.Clients.FullName;
                    AppConnect.model01.EventRegistrations.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Мероприятие", "Клиент " + name + " убран из списка " + currentEvent.Name);
                    UpdateMembers();
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
