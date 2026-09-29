using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace WPFbowlingKurs.AppData
{
    internal class CsvExporter
    {
        public static void Export<T>(IEnumerable<T> items, string defaultFileName)
        {
            if (items == null) return;

            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                FileName = defaultFileName
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var props = typeof(T).GetProperties();

                    List<string> headers = new List<string>();
                    foreach (var p in props) headers.Add($"\"{p.Name}\"");
                    sb.AppendLine(string.Join(";", headers));

                    foreach (var item in items)
                    {
                        List<string> cells = new List<string>();
                        foreach (var p in props)
                        {
                            var val = p.GetValue(item, null);
                            string str = val != null ? val.ToString() : "";
                            if (val is DateTime)
                            {
                                str = ((DateTime)val).ToString("dd.MM.yyyy HH:mm");
                            }
                            string cleanStr = str.Replace("\"", "\"\"");
                            cells.Add("\"" + cleanStr + "\"");
                        }
                        sb.AppendLine(string.Join(";", cells));
                    }

                    File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));
                    AppConnect.AddLog("Экспорт", "Экспорт в файл " + Path.GetFileName(dlg.FileName));

                    string awards = "";
                    try
                    {
                        awards = PointsHelper.AddPoints(AppConnect.currentUser, PointsHelper.PointsForExport);
                        AppConnect.model01.SaveChanges();
                    }
                    catch
                    {
                        awards = "";
                    }

                    MessageBox.Show("Данные успешно сохранены! +" + PointsHelper.PointsForExport + " балла\n" + awards,
                        "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
