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
    public partial class AddEditClientPage : Page
    {
        private Clients currentClient = new Clients();

        public AddEditClientPage(Clients selectedClient)
        {
            InitializeComponent();

            if (selectedClient != null)
            {
                currentClient = selectedClient;
                tbTitle.Text = "Редактирование клиента";
            }
            else
            {
                currentClient.RegistrationDate = DateTime.Today;
            }

            if (AppConnect.currentUser.Role == "Operator")
                tbPoints.IsEnabled = false;

            DataContext = currentClient;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentClient.FullName))
                errors.AppendLine("Введите ФИО");
            if (string.IsNullOrWhiteSpace(currentClient.Phone))
                errors.AppendLine("Введите телефон");
            else if (currentClient.Phone.Count(char.IsDigit) < 10)
                errors.AppendLine("В телефоне должно быть не меньше 10 цифр");
            if (!string.IsNullOrWhiteSpace(currentClient.Email) && !currentClient.Email.Contains("@"))
                errors.AppendLine("Email введён неправильно");
            if (currentClient.Points < 0)
                errors.AppendLine("Баллы не могут быть отрицательными");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isNew = currentClient.Id == 0;
            if (isNew)
                AppConnect.model01.Clients.Add(currentClient);

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(isNew ? "Добавление" : "Редактирование", "Клиент " + currentClient.FullName);
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
