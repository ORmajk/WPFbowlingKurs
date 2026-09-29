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
    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();

            var statuses = AppConnect.model01.InventoryStatuses.ToList();
            statuses.Insert(0, new InventoryStatuses { Name = "Все статусы" });
            cbStatus.ItemsSource = statuses;
            cbStatus.SelectedIndex = 0;

            if (AppConnect.currentUser.Role == "Operator")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            if (AppConnect.currentUser.Role != "Admin")
                btnDelete.Visibility = Visibility.Collapsed;
        }

        private void UpdateInventory()
        {
            var items = AppConnect.model01.InventoryItems.ToList();

            if (tbSearch.Text != "")
                items = items.Where(x => x.Name != null && x.Name.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

            if (cbStatus.SelectedIndex > 0)
            {
                var status = cbStatus.SelectedItem as InventoryStatuses;
                items = items.Where(x => x.StatusId == status.Id).ToList();
            }

            items = items.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
            dgInventory.ItemsSource = items;
            tbCount.Text = "Позиций: " + items.Count + ", всего штук: " + items.Sum(x => x.Quantity);
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppConnect.RefreshData();
                UpdateInventory();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateInventory();
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateInventory();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AddEditInventoryPage(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgInventory.SelectedItem as InventoryItems;
            if (selected == null)
            {
                MessageBox.Show("Выберите позицию!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.frameMain.Navigate(new AddEditInventoryPage(selected));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgInventory.SelectedItem as InventoryItems;
            if (selected == null)
            {
                MessageBox.Show("Выберите позицию!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Удалить " + selected.Name + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string name = selected.Name;
                    AppConnect.model01.InventoryItems.Remove(selected);
                    AppConnect.model01.SaveChanges();
                    AppConnect.AddLog("Удаление", "Удалён инвентарь " + name);
                    UpdateInventory();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + AppConnect.GetError(ex), "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    AppConnect.RefreshData();
                }
            }
        }

        private void btnGive_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgInventory.SelectedItem as InventoryItems;
            if (selected == null)
            {
                MessageBox.Show("Выберите позицию!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.InventoryStatuses.Name != "Доступен")
            {
                MessageBox.Show("Этот инвентарь сейчас недоступен!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected.Quantity <= 0)
            {
                MessageBox.Show("Нет в наличии!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selected.Quantity--;
            AppConnect.model01.SaveChanges();
            AppConnect.AddLog("Выдача инвентаря", selected.Name + ", осталось " + selected.Quantity);
            UpdateInventory();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgInventory.SelectedItem as InventoryItems;
            if (selected == null)
            {
                MessageBox.Show("Выберите позицию!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selected.Quantity++;
            AppConnect.model01.SaveChanges();
            AppConnect.AddLog("Возврат инвентаря", selected.Name + ", стало " + selected.Quantity);
            UpdateInventory();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
