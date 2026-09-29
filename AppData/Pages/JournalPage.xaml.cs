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
    public partial class JournalPage : Page
    {
        public JournalPage()
        {
            InitializeComponent();

            var users = AppConnect.model01.Users.ToList();
            users.Insert(0, new Users { FullName = "Все сотрудники" });
            cbUser.ItemsSource = users;
            cbUser.SelectedIndex = 0;
        }

        private void UpdateJournal()
        {
            var actions = AppConnect.model01.UserActions.ToList();

            if (cbUser.SelectedIndex > 0)
            {
                var user = cbUser.SelectedItem as Users;
                actions = actions.Where(x => x.UserId == user.Id).ToList();
            }

            dgActions.ItemsSource = actions.OrderByDescending(x => x.Timestamp).ToList();
        }

        private void cbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateJournal();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var actions = dgActions.ItemsSource as List<UserActions>;
            if (actions == null) return;

            var list = actions.Select(x => new
            {
                Время = x.Timestamp,
                Сотрудник = x.Users.FullName,
                Действие = x.ActionType,
                Подробности = x.Details
            }).ToList();

            CsvExporter.Export(list, "Журнал.csv");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
