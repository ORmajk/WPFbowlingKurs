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
    public partial class AddEditEventPage : Page
    {
        private Events currentEvent = new Events();

        public AddEditEventPage(Events selectedEvent)
        {
            InitializeComponent();

            cbLane.ItemsSource = AppConnect.model01.Lanes.OrderBy(x => x.LaneNumber).ToList();

            if (selectedEvent != null)
            {
                currentEvent = selectedEvent;
                tbTitle.Text = "Редактирование мероприятия";
            }
            else
            {
                currentEvent.EventDate = DateTime.Today.AddDays(7);
            }

            if (currentEvent.Lanes != null)
                cbLane.SelectedItem = currentEvent.Lanes;
            else
                chbAllLanes.IsChecked = true;

            DataContext = currentEvent;
        }

        private void chbAllLanes_Changed(object sender, RoutedEventArgs e)
        {
            cbLane.IsEnabled = chbAllLanes.IsChecked != true;
            if (chbAllLanes.IsChecked == true)
                cbLane.SelectedItem = null;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentEvent.Name))
            {
                MessageBox.Show("Сначала введите название мероприятия!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(tbDescription.Text) &&
                MessageBox.Show("Заменить текущее описание?", "AI-помощник", MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            string text = AiHelper.EventDescription(currentEvent.Name, currentEvent.EventDate, cbLane.SelectedItem as Lanes);
            if (text == "")
            {
                MessageBox.Show("AI-помощник пока не может ответить. Попробуйте позже.", "AI-помощник",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            currentEvent.Description = text;
            tbDescription.Text = currentEvent.Description;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentEvent.Name))
                errors.AppendLine("Введите название");
            if (currentEvent.EventDate == DateTime.MinValue)
                errors.AppendLine("Выберите дату");
            else if (currentEvent.Id == 0 && currentEvent.EventDate < DateTime.Today)
                errors.AppendLine("Дата мероприятия не может быть в прошлом");
            if (chbAllLanes.IsChecked != true && cbLane.SelectedItem == null)
                errors.AppendLine("Выберите дорожку или отметьте «На всех дорожках»");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (chbAllLanes.IsChecked == true)
            {
                currentEvent.Lanes = null;
                currentEvent.LaneId = null;
            }
            else
            {
                currentEvent.Lanes = cbLane.SelectedItem as Lanes;
            }

            bool isNew = currentEvent.Id == 0;
            if (isNew)
                AppConnect.model01.Events.Add(currentEvent);

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(isNew ? "Добавление" : "Редактирование", "Мероприятие " + currentEvent.Name);
                MessageBox.Show("Мероприятие сохранено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
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
