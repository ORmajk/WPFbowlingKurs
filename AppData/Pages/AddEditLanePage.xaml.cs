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
    public partial class AddEditLanePage : Page
    {
        private Lanes currentLane = new Lanes();

        public AddEditLanePage(Lanes selectedLane)
        {
            InitializeComponent();

            if (selectedLane != null)
            {
                currentLane = selectedLane;
                tbTitle.Text = "Редактирование дорожки №" + currentLane.LaneNumber;
            }

            DataContext = currentLane;
            cbType.ItemsSource = AppConnect.model01.LaneTypes.ToList();
            cbStatus.ItemsSource = AppConnect.model01.LaneStatuses.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (currentLane.LaneNumber <= 0)
                errors.AppendLine("Укажите номер дорожки (больше 0)");
            if (currentLane.LaneTypes == null)
                errors.AppendLine("Выберите категорию");
            if (currentLane.LaneStatuses == null)
                errors.AppendLine("Выберите состояние");
            if (currentLane.Capacity <= 0 || currentLane.Capacity > 20)
                errors.AppendLine("Вместимость должна быть от 1 до 20");
            if (currentLane.PricePerHour <= 0)
                errors.AppendLine("Укажите цену за час");

            int number = currentLane.LaneNumber;
            int id = currentLane.Id;
            if (AppConnect.model01.Lanes.Any(x => x.LaneNumber == number && x.Id != id))
                errors.AppendLine("Дорожка с таким номером уже есть");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentLane.Id == 0)
                AppConnect.model01.Lanes.Add(currentLane);

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(id == 0 ? "Добавление" : "Редактирование", "Дорожка №" + currentLane.LaneNumber);
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
