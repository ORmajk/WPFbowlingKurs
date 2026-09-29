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
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLogin.Text) || string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var userObj = AppConnect.model01.Users.FirstOrDefault(x => x.Login == tbLogin.Text && x.Password == pbPassword.Password);

                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя нет! Проверьте логин и пароль.", "Ошибка при авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AppConnect.currentUser = userObj;
                AppConnect.AddLog("Вход", "Успешная авторизация: " + userObj.Login);

                MessageBox.Show("Здравствуйте, " + userObj.FullName + "!", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                if (userObj.Role == "Client")
                    AppFrame.frameMain.Navigate(new ClientMenuPage());
                else
                    AppFrame.frameMain.Navigate(new MenuPage());

                tbLogin.Text = "";
                pbPassword.Password = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + AppConnect.GetError(ex), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new RegistrationPage());
        }
    }
}
