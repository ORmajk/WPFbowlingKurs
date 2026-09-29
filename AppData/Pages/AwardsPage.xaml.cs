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
    public partial class AwardsPage : Page
    {
        public AwardsPage()
        {
            InitializeComponent();

            if (AppConnect.currentUser.Role != "Admin")
            {
                btnAddAward.IsEnabled = false;
                btnDeleteAward.IsEnabled = false;
                btnTake.Visibility = Visibility.Collapsed;
            }

            StringBuilder rules = new StringBuilder("Автоматически: ");
            foreach (var r in PointsHelper.Rules)
                rules.Append("«" + r.Name + "» - от " + r.NeedPoints + " баллов; ");
            rules.Append("«Участник мероприятий» - за запись на мероприятие. Баллы: бронь +" + PointsHelper.PointsForBooking +
                ", мероприятие +" + PointsHelper.PointsForEvent + ", экспорт +" + PointsHelper.PointsForExport + ".");
            tbRules.Text = rules.ToString();
        }

        private void UpdateData()
        {
            var users = AppConnect.model01.Users.OrderBy(x => x.FullName).ToList();
            var awards = AppConnect.model01.Awards.OrderBy(x => x.Name).ToList();

            cbUser.ItemsSource = users;
            cbAward.ItemsSource = awards;
            dgAwards.ItemsSource = awards;
            dgRating.ItemsSource = users.OrderByDescending(x => x.Points).ToList();
            dgUserAwards.ItemsSource = AppConnect.model01.UserAwards.OrderByDescending(x => x.AwardedAt).ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateData();
            }
        }

        private void btnGive_Click(object sender, RoutedEventArgs e)
        {
            var user = cbUser.SelectedItem as Users;
            var award = cbAward.SelectedItem as Awards;
            if (user == null || award == null)
            {
                MessageBox.Show("Выберите пользователя и награду!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (user.UserAwards.Any(x => x.AwardId == award.Id))
            {
                MessageBox.Show("У этого пользователя уже есть такая награда!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                PointsHelper.GiveAward(user, award.Name, award.Description);
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("Награда", "Выдана награда «" + award.Name + "» пользователю " + user.Login);
                MessageBox.Show("Награда выдана!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnTake_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUserAwards.SelectedItem as UserAwards;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Забрать награду «" + selected.Awards.Name + "» у " + selected.Users.FullName + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string text = selected.Awards.Name + " у " + selected.Users.Login;
                    AppConnect.model01.UserAwards.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Награда", "Забрана награда " + text);
                    UpdateData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnAddAward_Click(object sender, RoutedEventArgs e)
        {
            string name = tbAwardName.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("Введите название награды!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (AppConnect.model01.Awards.Any(x => x.Name == name))
            {
                MessageBox.Show("Такая награда уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Awards award = new Awards();
                award.Name = name;
                award.Description = tbAwardDescription.Text.Trim();
                AppConnect.model01.Awards.Add(award);
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("Добавление", "Добавлена награда " + name);
                tbAwardName.Text = "";
                tbAwardDescription.Text = "";
                UpdateData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnDeleteAward_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgAwards.SelectedItem as Awards;
            if (selected == null)
            {
                MessageBox.Show("Выберите награду!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.UserAwards.Count > 0)
            {
                MessageBox.Show("Эту награду уже получили пользователи, удалить нельзя!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Удалить награду «" + selected.Name + "»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string name = selected.Name;
                    AppConnect.model01.Awards.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалена награда " + name);
                    UpdateData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var users = dgRating.ItemsSource as List<Users>;
            if (users == null) return;

            var list = users.Select(x => new
            {
                ФИО = x.FullName,
                Логин = x.Login,
                Роль = AppConnect.GetRoleName(x.Role),
                Баллы = x.Points,
                Наград = x.UserAwards.Count
            }).ToList();

            CsvExporter.Export(list, "Рейтинг.csv");
            UpdateData();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
