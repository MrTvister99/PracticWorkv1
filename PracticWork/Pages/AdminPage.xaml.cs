using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using PracticWork.Window;
using Microsoft.Office.Interop.Excel;

namespace PracticWork.Pages
{
    public partial class AdminPage : System.Windows.Controls.Page
    {
        public int i;
        public static ObservableCollection<application> TowarList = new ObservableCollection<application>();
        public ObservableCollection<string> StatusOptions { get; set; }
        public string Name1;
        string server = "DESKTOP-9MU0DUB";
        string database = "practicWork";
        string username = "MrTv";
        string passwordDB = "1";
        List<string> values;
        public AdminPage()
        {
            InitializeComponent();
            System.Windows.Application.Current.MainWindow.Title = "Страница администратора ";
            podkl();
            comboBoxStatus.Background = Brushes.Yellow;

            StatusOptions = new ObservableCollection<string>
        {
            "Ожидает",
            "Обработка",
            "Отклонен",
            "Одобрен",
            "Готов",

        };
            comboBoxStatus.ItemsSource = StatusOptions;
        }
        public void ExportToExcel(object sender, RoutedEventArgs e)
        {


            // Создание нового Excel-файла
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                // Заголовки столбцов
                worksheet.Cell(currentRow, 1).Value = "Telephone";
                worksheet.Cell(currentRow, 2).Value = "DateAdd";
                worksheet.Cell(currentRow, 3).Value = "Product";
                worksheet.Cell(currentRow, 4).Value = "Count";
                worksheet.Cell(currentRow, 5).Value = "ClientName";
                worksheet.Cell(currentRow, 6).Value = "Status";

                foreach (var item in TowarList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Telephone;
                    worksheet.Cell(currentRow, 2).Value = item.DateAdd;
                    worksheet.Cell(currentRow, 3).Value = item.equipment;
                    worksheet.Cell(currentRow, 4).Value = item.Count;
                    worksheet.Cell(currentRow, 5).Value = item.UsersFIO;
                    worksheet.Cell(currentRow, 6).Value = item.Status;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string filePath = System.IO.Path.Combine(desktopPath, "Orders.xlsx");
                workbook.SaveAs(filePath);
                Console.WriteLine($"Данные экспортированы в файл: {filePath}");
            }
        }

        private void podkl()
        {

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM Orders";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TowarList.Add(new application
                            {
                                Telephone = reader["Telephone"].ToString(),
                                DateAdd = DateTime.Parse(reader["DateAdd"].ToString()).ToString("dd.MM.yyyy"),
                                equipment = reader["Product"].ToString(),
                                Count = (int)reader["Count"],
                                UsersFIO = reader["NameClient"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            TowarListView.ItemsSource = TowarList;


            var uniqueEquipments = TowarList.Select(t => t.equipment).Distinct().ToList();
            uniqueEquipments.Insert(0, "Все");

            comboBoxEquipment.ItemsSource = uniqueEquipments;
            comboBoxEquipment.SelectionChanged += ComboBoxEquipment;
        }
        private void ComboBoxEquipment(object sender, SelectionChangedEventArgs e)
        {

            string selectedEquipment = comboBoxEquipment.SelectedItem as string;


            if (selectedEquipment == "Все")
            {
                TowarListView.ItemsSource = TowarList;
            }
            else
            {

                var filteredList = TowarList.Where(t => t.equipment == selectedEquipment).ToList();

                // Обновление источника данных для ListView
                TowarListView.ItemsSource = filteredList;
            }
        }

        private async void ComboBoxStatus(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(500);
            string selectedStatus = comboBoxStatus.Text;


            application selectedApplication = TowarListView.SelectedItem as application;

            if (selectedApplication != null && selectedStatus != null)
            {

                UpdateStatusInDatabase(selectedApplication, selectedStatus);
                selectedApplication.Status = selectedStatus;
                TowarListView.Items.Refresh();
            }
        }
        private void UpdateStatusInDatabase(application selectedApplication, string Selectionstatus)
        {
            string newStatus = Selectionstatus;

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "UPDATE Orders SET Status = @Status WHERE NameClient = @NameClient";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@NameClient", selectedApplication.UsersFIO);

                    command.ExecuteNonQuery();
                }
            }
        }
        private void OldCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            ApplyFilter(true);
        }

        private void OldCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(false);
        }

        private void NewCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            ApplyFilter(false);
        }

        private void NewCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(true);
        }

        private void ApplyFilter(bool isOld)
        {
            if (isOld)
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderByDescending(a => DateTime.Parse(a.DateAdd)));
                TowarListView.ItemsSource = sortedList;
            }
            else
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderBy(a => DateTime.Parse(a.DateAdd)));
                TowarListView.ItemsSource = sortedList;
            }
        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {

            if (TowarListView.SelectedItem is application selectedApplication)
            {

                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Удаляем запись из базы данных и из списка
                    RemoveApplication(selectedApplication);
                }
            }
        }

        private void RemoveApplication(application app)
        {

            DeleteApplicationFromDatabase(app);


            TowarList.Remove(app);

            // Обновляем источник данных для обновления списка в интерфейсе
            TowarListView.ItemsSource = null;
            TowarListView.ItemsSource = TowarList;
        }

        private void DeleteApplicationFromDatabase(application app)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();
                string sql = "DELETE FROM Orders WHERE NameClient  = @FIO";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FIO", app.UsersFIO);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void myTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = myTextBox1.Text;

            var filteredItems = TowarList.Where(item => item.UsersFIO.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            TowarListView.ItemsSource = filteredItems;
        }
        private void ComboB()
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM Group1";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        List<string> values = new List<string>();
                        ComboBoxData.ClearItems(values);
                        while (reader.Read())
                        {
                            string Name2 = reader["Predmet"].ToString();
                            if (!values.Contains(Name2))
                            {
                                values.Add(Name2);
                                ComboBoxData.AddItems(values);
                            }
                        }
                    }
                    connection.Close();
                }
            }
        }
        private void comboBox1_SelectionChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                var comboBox = (ComboBox)sender;
                var selectedItem = comboBox.SelectedItem;
                connection.Open();



                string sql = $"INSERT INTO Group1 (Predmet,Name)  VALUES (@Predmet,@Name)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    ComboBox comboBox1 = (ComboBox)sender;
                    int index = TowarListView.Items.IndexOf(comboBox.DataContext);
                    string listName = comboBox.DataContext.ToString();

                    MessageBox.Show($"Selected item: {selectedItem}, index: {listName}");
                    if (selectedItem != null)
                    {
                        command.Parameters.AddWithValue("@Predmet", $"{selectedItem}");
                        command.Parameters.AddWithValue("@Name", $"{listName}");



                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();





            }
        }
        public class application
        {
            public string Telephone { get; set; }
            public string DateAdd { get; set; }
            public string equipment { get; set; }
            public int Count { get; set; }
            public string UsersFIO { get; set; }
            public string Status { get; set; }

        }
        public static class ComboBoxData
        {

            public static ObservableCollection<string> Items { get; } = new ObservableCollection<string>
            {

            };

            public static void AddItems(List<string> items)
            {
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }

            public static void ClearItems(List<string> items)
            {
                Items.Clear();
            }

        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            mainWindow.NavigateToLoginPage();
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (TowarListView.SelectedItem is application selectedApplication)
            {
                // Открываем окно редактирования с выбранным элементом
                var window = new WindowEdit(selectedApplication);
                bool? result = window.ShowDialog();

                if (result == true)
                {
                    EditApplicationInDatabase(selectedApplication);
                    if (TowarListView.ItemsSource is ObservableCollection<application> collection)
                    {
                        collection.Clear();
                    }
                    podkl();
                }
            }
            else
            {
                // Обработка случая, когда выбранный элемент не существует
                MessageBox.Show("Выберите элемент для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditApplicationInDatabase(application app)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();
                string sql = "UPDATE Orders SET DateAdd = @DateAdd,Telephone=@Telephone, Product = @Product, Count = @Count, NameClient = @NameClient, Status = @Status WHERE NameClient = @NameClient";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DateAdd", app.DateAdd);
                    command.Parameters.AddWithValue("@Telephone", app.Telephone);
                    command.Parameters.AddWithValue("@Product", app.equipment);
                    command.Parameters.AddWithValue("@Count", app.Count);
                    command.Parameters.AddWithValue("@NameClient", app.UsersFIO);
                    command.Parameters.AddWithValue("@Status", app.Status);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
