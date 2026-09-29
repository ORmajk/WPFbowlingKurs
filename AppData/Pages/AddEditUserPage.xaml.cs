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
    public partial class AddEditUserPage : Page
    {
        private Users currentUser = new Users();

        public AddEditUserPage(Users selectedUser)
        {
            InitializeComponent();

            if (selectedUser != null)
            {
                currentUser = selectedUser;
                tbTitle.Text = "Редактирование сотрудника";
            }
            else
            {
                currentUser.Role = "Operator";
            }

            cbRole.ItemsSource = new List<string> { "Admin", "Manager", "Operator" };
            DataContext = currentUser;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentUser.FullName))
                errors.AppendLine("Введите ФИО");
            if (string.IsNullOrWhiteSpace(currentUser.Login) || currentUser.Login.Length < 3)
                errors.AppendLine("Логин должен быть не короче 3 символов");
            if (string.IsNullOrWhiteSpace(currentUser.Password) || currentUser.Password.Length < 6)
                errors.AppendLine("Пароль должен быть не короче 6 символов");
            if (string.IsNullOrWhiteSpace(currentUser.Role))
                errors.AppendLine("Выберите роль");
            if (currentUser.Points < 0)
                errors.AppendLine("Баллы не могут быть отрицательными");

            string login = currentUser.Login;
            int id = currentUser.Id;
            if (AppConnect.model01.Users.Any(x => x.Login == login && x.Id != id))
                errors.AppendLine("Такой логин уже занят");

            if (currentUser.Id == AppConnect.currentUser.Id && currentUser.Role != "Admin")
                errors.AppendLine("Нельзя убрать у себя роль администратора");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isNew = currentUser.Id == 0;
            if (isNew)
                AppConnect.model01.Users.Add(currentUser);

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(isNew ? "Добавление" : "Редактирование", "Сотрудник " + currentUser.Login);
                MessageBox.Show("Информация сохранена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + AppConnect.GetError(ex), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
