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
    public partial class AddEditInventoryPage : Page
    {
        private InventoryItems currentItem = new InventoryItems();

        public AddEditInventoryPage(InventoryItems selectedItem)
        {
            InitializeComponent();

            if (selectedItem != null)
            {
                currentItem = selectedItem;
                tbTitle.Text = "Редактирование инвентаря";
            }

            List<string> types = new List<string> { "Шар", "Ботинки", "Защита" };
            foreach (var t in AppConnect.model01.InventoryItems.Select(x => x.Type).Distinct().ToList())
            {
                if (!types.Contains(t)) types.Add(t);
            }
            cbType.ItemsSource = types;
            cbStatus.ItemsSource = AppConnect.model01.InventoryStatuses.ToList();

            DataContext = currentItem;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentItem.Name))
                errors.AppendLine("Введите название");
            if (string.IsNullOrWhiteSpace(currentItem.Type))
                errors.AppendLine("Выберите тип");
            if (currentItem.InventoryStatuses == null)
                errors.AppendLine("Выберите статус");
            if (currentItem.Quantity < 0)
                errors.AppendLine("Количество не может быть отрицательным");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isNew = currentItem.Id == 0;
            if (isNew)
            {
                currentItem.CreatedAt = DateTime.Now;
                AppConnect.model01.InventoryItems.Add(currentItem);
            }

            try
            {
                AppConnect.model01.SaveChanges();
                AppConnect.AddLog(isNew ? "Добавление" : "Редактирование", "Инвентарь " + currentItem.Name);
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
