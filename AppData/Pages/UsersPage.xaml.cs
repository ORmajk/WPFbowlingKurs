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
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                dgUsers.ItemsSource = AppConnect.model01.Users.Where(x => x.Role != "Client").OrderBy(x => x.FullName).ToList();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditUserPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditUserPage(selected));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.Id == AppConnect.currentUser.Id)
            {
                MessageBox.Show("Нельзя удалить самого себя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (selected.UserActions.Count > 0)
            {
                MessageBox.Show("Нельзя удалить сотрудника, у него есть записи в журнале действий!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Удалить сотрудника " + selected.FullName + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string login = selected.Login;
                    AppConnect.model01.UserAwards.RemoveRange(selected.UserAwards.ToList());
                    AppConnect.model01.Users.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалён сотрудник " + login);
                    dgUsers.ItemsSource = AppConnect.model01.Users.Where(x => x.Role != "Client").OrderBy(x => x.FullName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + AppConnect.GetError(ex), "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
