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
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            string fullName = tbFullName.Text.Trim();
            string phone = tbPhone.Text.Trim();
            string email = tbEmail.Text.Trim();
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password;

            if (fullName == "")
                errors.AppendLine("Введите ФИО");
            if (phone == "")
                errors.AppendLine("Введите телефон");
            else if (phone.Count(char.IsDigit) < 10)
                errors.AppendLine("В телефоне должно быть не меньше 10 цифр");
            if (email != "" && (!email.Contains("@") || !email.Contains(".")))
                errors.AppendLine("Email введён неправильно");
            if (login == "")
                errors.AppendLine("Введите логин");
            else if (login.Length < 3)
                errors.AppendLine("Логин должен быть не короче 3 символов");
            if (password.Length < 6)
                errors.AppendLine("Пароль должен быть не короче 6 символов");
            else if (!password.Any(char.IsDigit))
                errors.AppendLine("В пароле должна быть хотя бы одна цифра");
            if (password != pbPassword2.Password)
                errors.AppendLine("Пароли не совпадают");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (AppConnect.model01.Users.Any(x => x.Login == login))
                {
                    MessageBox.Show("Такой логин уже занят, придумайте другой!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (AppConnect.FindClientUserByPhone(phone) != null)
                {
                    MessageBox.Show("Пользователь с таким телефоном уже зарегистрирован!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Users user = new Users();
                user.FullName = fullName;
                user.Phone = phone;
                user.Email = email == "" ? null : email;
                user.Login = login;
                user.Password = password;
                user.Role = "Client";
                user.Points = 0;
                user.IsSubscribedToAI = false;

                var client = AppConnect.FindClientByPhone(phone);
                if (client != null)
                {
                    user.Points = client.Points;
                    user.IsSubscribedToAI = client.IsSubscribedToAl;
                }
                else
                {
                    client = new Clients();
                    client.FullName = fullName;
                    client.Phone = phone;
                    client.Email = user.Email;
                    client.RegistrationDate = DateTime.Today;
                    client.Points = 0;
                    client.IsSubscribedToAl = false;
                    AppConnect.model01.Clients.Add(client);
                }

                AppConnect.model01.Users.Add(user);
                AppConnect.model01.SaveChanges();

                try
                {
                    AppConnect.model01.Database.ExecuteSqlCommand(
                        "UPDATE Clients SET Login = @p0, Password = @p1 WHERE Id = @p2", login, password, client.Id);
                }
                catch
                {
                }

                AppConnect.currentUser = user;
                AppConnect.AddLog("Регистрация", "Зарегистрирован клиент " + login);

                MessageBox.Show("Регистрация прошла успешно! Добро пожаловать, " + fullName + "!", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frameMain.Navigate(new ClientMenuPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + AppConnect.GetError(ex), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
