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
    public partial class EventsPage : Page
    {
        public EventsPage()
        {
            InitializeComponent();

            if (AppConnect.currentUser.Role == "Operator")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            if (AppConnect.currentUser.Role != "Admin" && AppConnect.currentUser.Role != "Manager")
                btnDelete.Visibility = Visibility.Collapsed;
        }

        private void UpdateEvents()
        {
            var events = AppConnect.model01.Events.ToList();

            if (tbSearch.Text != "")
                events = events.Where(x => x.Name.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

            if (chbFuture.IsChecked == true)
                events = events.Where(x => x.EventDate >= DateTime.Today).ToList();

            events = events.OrderByDescending(x => x.EventDate).ToList();
            dgEvents.ItemsSource = events;
            tbCount.Text = "Найдено: " + events.Count;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateEvents();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEvents();
        }

        private void chbFuture_Changed(object sender, RoutedEventArgs e)
        {
            UpdateEvents();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditEventPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgEvents.SelectedItem as Events;
            if (selected == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditEventPage(selected));
        }

        private void dgEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgEvents.SelectedItem != null)
                btnMembers_Click(sender, null);
        }

        private void btnMembers_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgEvents.SelectedItem as Events;
            if (selected == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new EventMembersPage(selected));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgEvents.SelectedItem as Events;
            if (selected == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string text = "Удалить мероприятие «" + selected.Name + "»?";
            if (selected.EventRegistrations.Count > 0)
                text += "\nНа него записано участников: " + selected.EventRegistrations.Count + ", их записи тоже удалятся.";

            if (MessageBox.Show(text, "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string name = selected.Name;
                    AppConnect.model01.EventRegistrations.RemoveRange(selected.EventRegistrations.ToList());
                    AppConnect.model01.Events.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалено мероприятие " + name);
                    MessageBox.Show("Мероприятие удалено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppConnect.RefreshData();
                    UpdateEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + AppConnect.GetError(ex), "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var events = dgEvents.ItemsSource as List<Events>;
            if (events == null) return;

            var list = events.Select(x => new
            {
                Название = x.Name,
                Дата = x.EventDate,
                Дорожка = x.Lanes != null ? x.Lanes.LaneNumber.ToString() : "все",
                Участников = x.EventRegistrations.Count,
                Описание = x.Description
            }).ToList();

            CsvExporter.Export(list, "Мероприятия.csv");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
