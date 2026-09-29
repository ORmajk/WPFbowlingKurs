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
    public partial class AiPage : Page
    {
        public AiPage()
        {
            InitializeComponent();

            dpFreeDate.SelectedDate = DateTime.Today;
            dpFreeDate.DisplayDateStart = DateTime.Today;

            if (AppConnect.currentUser.Role == "Client")
            {
                tiLoad.Visibility = Visibility.Collapsed;
                tiForecast.Visibility = Visibility.Collapsed;
                tiAdvice.Visibility = Visibility.Collapsed;
                tiEvent.Visibility = Visibility.Collapsed;
                tcAi.SelectedItem = tiFree;
            }
        }

        private void CheckSubscription()
        {
            var user = AppConnect.currentUser;
            if (user.IsSubscribedToAI)
            {
                brdNoAi.Visibility = Visibility.Collapsed;
                tcAi.Visibility = Visibility.Visible;
                cbEvent.ItemsSource = AppConnect.model01.Events.OrderByDescending(x => x.EventDate).ToList();
            }
            else
            {
                brdNoAi.Visibility = Visibility.Visible;
                tcAi.Visibility = Visibility.Collapsed;
                tbNoAi.Text = "Помощник анализирует загруженность клуба, подсказывает свободное время, подбирает инвентарь " +
                    "и пишет описания мероприятий.\nСтоимость: " + PointsHelper.PriceAI + " баллов, у вас " + user.Points + ".";
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                CheckSubscription();
            }
        }

        private void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            var user = AppConnect.currentUser;
            if (user.Points < PointsHelper.PriceAI)
            {
                MessageBox.Show("Не хватает баллов! Нужно " + PointsHelper.PriceAI + ", у вас " + user.Points + ".\n" +
                    "Баллы начисляются за бронирования и участие в мероприятиях.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                PointsHelper.SpendPoints(user, PointsHelper.PriceAI);
                user.IsSubscribedToAI = true;
                var client = AppConnect.GetCurrentClient();
                if (client != null)
                    client.IsSubscribedToAl = true;
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("AI-помощник", "Подключён AI-помощник за баллы");
                MessageBox.Show("AI-помощник подключён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckSubscription();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            tbLoad.Text = AiHelper.AnalyzeLoad();
            AppConnect.AddLog("AI-помощник", "Анализ загруженности");
        }

        private void btnForecast_Click(object sender, RoutedEventArgs e)
        {
            tbForecast.Text = AiHelper.Forecast();
            AppConnect.AddLog("AI-помощник", "Прогноз посещаемости");
        }

        private void btnAdvice_Click(object sender, RoutedEventArgs e)
        {
            tbAdvice.Text = AiHelper.Recommendations();
            AppConnect.AddLog("AI-помощник", "Рекомендации по расписанию");
        }

        private void btnFree_Click(object sender, RoutedEventArgs e)
        {
            if (dpFreeDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tbFree.Text = AiHelper.FreeTime(dpFreeDate.SelectedDate.Value);
        }

        private void btnPick_Click(object sender, RoutedEventArgs e)
        {
            int weight, age, shoe;
            StringBuilder errors = new StringBuilder();
            if (!int.TryParse(tbWeight.Text, out weight) || weight < 15 || weight > 200)
                errors.AppendLine("Вес введите числом от 15 до 200");
            if (!int.TryParse(tbAge.Text, out age) || age < 3 || age > 100)
                errors.AppendLine("Возраст введите числом от 3 до 100");
            if (!int.TryParse(tbShoe.Text, out shoe) || shoe < 20 || shoe > 50)
                errors.AppendLine("Размер обуви введите числом от 20 до 50");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            tbPick.Text = AiHelper.PickInventory(weight, age, shoe);
        }

        private void btnDescribe_Click(object sender, RoutedEventArgs e)
        {
            var ev = cbEvent.SelectedItem as Events;
            if (ev == null)
            {
                MessageBox.Show("Выберите мероприятие!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string text = AiHelper.EventDescription(ev.Name, ev.EventDate, ev.Lanes);
            if (text == "")
                tbDescribe.Text = "AI-помощник пока не может ответить. Попробуйте позже.";
            else
                tbDescribe.Text = text;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var ev = cbEvent.SelectedItem as Events;
            if (ev == null || string.IsNullOrWhiteSpace(tbDescribe.Text) || tbDescribe.Text.StartsWith("AI-помощник пока"))
            {
                MessageBox.Show("Выберите мероприятие и сгенерируйте описание!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (AppConnect.currentUser.Role == "Operator")
            {
                MessageBox.Show("Оператор не может менять мероприятия.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string text = tbDescribe.Text.Trim();
                if (text.Length > 500) text = text.Substring(0, 500);
                ev.Description = text;
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog("AI-помощник", "Описание мероприятия " + ev.Name + " обновлено");
                MessageBox.Show("Описание сохранено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + AppConnect.GetError(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AppConnect.RefreshData();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
