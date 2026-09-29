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
    public partial class MyAwardsPage : Page
    {
        public class AwardCard
        {
            public string Icon { get; set; }
            public string Name { get; set; }
            public string Info { get; set; }
            public string Back { get; set; }
        }

        public MyAwardsPage()
        {
            InitializeComponent();
        }

        private void LoadInfo()
        {
            var user = AppConnect.currentUser;

            try
            {
                string newAwards = PointsHelper.CheckAwards(user);
                if (newAwards != "")
                {
                    AppConnect.model01.SaveChanges();
                    MessageBox.Show(newAwards, "Новая награда!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                AppConnect.RefreshData();
            }

            tbPoints.Text = "У вас " + user.Points + " баллов";

            var next = PointsHelper.NextRule(user.Points);
            if (next != null)
            {
                tbNext.Text = "До награды «" + next.Name + "» осталось " + (next.NeedPoints - user.Points) + " баллов";
                pbNext.Value = user.Points * 100.0 / next.NeedPoints;
            }
            else
            {
                tbNext.Text = "Вы получили все награды за баллы!";
                pbNext.Value = 100;
            }

            if (user.IsSubscribedToAI)
            {
                tbAi.Text = "AI-помощник подключён";
                btnAi.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbAi.Text = "AI-помощник не подключён";
                btnAi.Visibility = Visibility.Visible;
            }

            List<AwardCard> cards = new List<AwardCard>();
            foreach (var award in AppConnect.model01.Awards.OrderBy(x => x.Id).ToList())
            {
                var my = user.UserAwards.FirstOrDefault(x => x.AwardId == award.Id);
                AwardCard card = new AwardCard();
                card.Name = award.Name;
                if (my != null)
                {
                    card.Icon = "🏆";
                    card.Info = award.Description + "\nПолучена " + my.AwardedAt.ToString("dd.MM.yyyy");
                    card.Back = "White";
                }
                else
                {
                    card.Icon = "🔒";
                    var rule = PointsHelper.Rules.FirstOrDefault(x => x.Name == award.Name);
                    card.Info = award.Description + (rule != null ? "\nНужно " + rule.NeedPoints + " баллов" : "");
                    card.Back = "#FFE6E6E6";
                }
                cards.Add(card);
            }
            lvAwards.ItemsSource = cards;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && AppConnect.currentUser != null)
            {
                AppConnect.RefreshData();
                LoadInfo();
            }
        }

        private void btnAi_Click(object sender, RoutedEventArgs e)
        {
            var user = AppConnect.currentUser;
            if (user.Points < PointsHelper.PriceAI)
            {
                MessageBox.Show("Не хватает баллов! Нужно " + PointsHelper.PriceAI + ", у вас " + user.Points + ".", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Подключить AI-помощника за " + PointsHelper.PriceAI + " баллов?", "AI-помощник",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

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
                LoadInfo();
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
