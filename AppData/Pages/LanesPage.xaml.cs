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
    public partial class LanesPage : Page
    {
        public LanesPage()
        {
            InitializeComponent();

            var types = AppConnect.model01.LaneTypes.ToList();
            types.Insert(0, new LaneTypes { Name = "Все категории" });
            cbType.ItemsSource = types;
            cbType.SelectedIndex = 0;

            var statuses = AppConnect.model01.LaneStatuses.ToList();
            statuses.Insert(0, new LaneStatuses { Name = "Все состояния" });
            cbStatus.ItemsSource = statuses;
            cbStatus.SelectedIndex = 0;

            cbSort.SelectedIndex = 0;

            if (AppConnect.currentUser.Role == "Operator")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            if (AppConnect.currentUser.Role != "Admin")
            {
                btnDelete.Visibility = Visibility.Collapsed;
            }
            if (AppConnect.currentUser.Role == "Client")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnExport.Visibility = Visibility.Collapsed;
                btnBook.Visibility = Visibility.Visible;
            }
        }

        private void UpdateLanes()
        {
            var lanes = AppConnect.model01.Lanes.ToList();

            if (tbSearch.Text != "")
                lanes = lanes.Where(x => x.LaneNumber.ToString().Contains(tbSearch.Text)).ToList();

            if (cbType.SelectedIndex > 0)
            {
                var type = cbType.SelectedItem as LaneTypes;
                lanes = lanes.Where(x => x.LaneTypeId == type.Id).ToList();
            }

            if (cbStatus.SelectedIndex > 0)
            {
                var status = cbStatus.SelectedItem as LaneStatuses;
                lanes = lanes.Where(x => x.StatusId == status.Id).ToList();
            }

            switch (cbSort.SelectedIndex)
            {
                case 1:
                    lanes = lanes.OrderBy(x => x.PricePerHour).ToList();
                    break;
                case 2:
                    lanes = lanes.OrderByDescending(x => x.PricePerHour).ToList();
                    break;
                case 3:
                    lanes = lanes.OrderByDescending(x => x.Capacity).ToList();
                    break;
                default:
                    lanes = lanes.OrderBy(x => x.LaneNumber).ToList();
                    break;
            }

            lvLanes.ItemsSource = lanes;
            tbCount.Text = "Найдено: " + lanes.Count;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateLanes();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLanes();
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLanes();
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLanes();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLanes();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditLanePage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvLanes.SelectedItem as Lanes;
            if (selected == null)
            {
                MessageBox.Show("Выберите дорожку для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditLanePage(selected));
        }

        private void lvLanes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvLanes.SelectedItem == null) return;
            if (AppConnect.currentUser.Role == "Client")
                btnBook_Click(sender, null);
            else if (AppConnect.currentUser.Role != "Operator")
                btnEdit_Click(sender, null);
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvLanes.SelectedItem as Lanes;
            if (selected == null)
            {
                MessageBox.Show("Выберите дорожку!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.LaneStatuses.Name == "Требует обслуживания")
            {
                MessageBox.Show("Эта дорожка на обслуживании, выберите другую!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new ClientBookingPage(selected));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvLanes.SelectedItem as Lanes;
            if (selected == null)
            {
                MessageBox.Show("Выберите дорожку для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Bookings.Count > 0 || selected.Events.Count > 0)
            {
                MessageBox.Show("Нельзя удалить дорожку, по ней есть бронирования или мероприятия!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Удалить дорожку №" + selected.LaneNumber + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int number = selected.LaneNumber;
                    AppConnect.model01.Lanes.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалена дорожка №" + number);
                    MessageBox.Show("Дорожка удалена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppConnect.RefreshData();
                    UpdateLanes();
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
            var lanes = lvLanes.ItemsSource as List<Lanes>;
            if (lanes == null) return;

            var list = lanes.Select(x => new
            {
                Номер = x.LaneNumber,
                Категория = x.LaneTypes.Name,
                Состояние = x.LaneStatuses.Name,
                Вместимость = x.Capacity,
                ЦенаЗаЧас = x.PricePerHour
            }).ToList();

            CsvExporter.Export(list, "Дорожки.csv");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
