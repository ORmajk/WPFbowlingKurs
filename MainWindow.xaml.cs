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

namespace WPFbowlingKurs
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppData.AppConnect.model01 = new AppData.BowlingClubEntities();
            AppData.AppFrame.frameMain = Frame1;
            Frame1.Navigate(new AppData.Pages.Authorization());
        }

        private void Frame1_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is AppData.Pages.Authorization)
            {
                while (Frame1.CanGoBack)
                    Frame1.RemoveBackEntry();
            }
        }
    }
}
