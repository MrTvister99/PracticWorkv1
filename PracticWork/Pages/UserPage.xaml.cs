using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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

namespace PracticWork.Pages
{

    public partial class UserPage : Page
    {
        string server = "DESKTOP-9MU0DUB";
        string database = "practicWork";
        string username = "MrTv";
        string passwordDB = "1";
        private ObservableCollection<application> TowarList = new ObservableCollection<application>();
        public ObservableCollection<string> OrderOptions { get; set; }
        public UserPage()
        {
            InitializeComponent();
            podkl();
            Application.Current.MainWindow.Title = "Главное меню";

            ComboBox();
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Telephone.Text) && 
                !string.IsNullOrEmpty(comboBox.Text) && ClientNameTextBox.Text != ""&&Count.Text!="")
            {
                using (SqlConnection connection = new SqlConnection
                    ($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
                {

                    connection.Open();

                    string sql = "INSERT INTO Orders " +
                        "(Telephone,DateAdd, Product,Count, NameClient, Status,Login) " +
                        "VALUES (@Telephone,@DateAdd,@Product,@Count, @UsersFIO, @Status,@Login)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Telephone", Telephone.Text);
                        command.Parameters.AddWithValue("@DateAdd", DateTime.Now);
                        command.Parameters.AddWithValue("@Product", comboBox.Text);
                        command.Parameters.AddWithValue("@Count", Count.Text);
                        command.Parameters.AddWithValue("@UsersFIO", ClientNameTextBox.Text);
                        command.Parameters.AddWithValue("@Status", "Новый");
                        if(string.IsNullOrEmpty(LoginPage.login))
                        {
                            command.Parameters.AddWithValue("@Login",RegistrationPage.loginUser);
                            
                        }
                        else{

                        command.Parameters.AddWithValue("@Login", LoginPage.login);
                        }

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                MessageBox.Show($"Поля не должны быть пустыми");
            }

        }

        private void podkl()
        {
            string loginToFilter = string.IsNullOrEmpty(LoginPage.login) ? RegistrationPage.loginUser : LoginPage.login;

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM Orders WHERE Login = @Login";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Login", loginToFilter);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TowarList.Add(new application
                            {
                                Telephone = reader["Telephone"].ToString(),
                                DateAdd = DateTime.Parse(reader["DateAdd"].ToString()).ToString("dd.MM.yyyy"),
                                equipment = reader["Product"].ToString(),
                                Count = "X" + (int)reader["Count"],
                                UsersFIO = reader["NameClient"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            TowarListView.ItemsSource = TowarList;
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.NavigateToLoginPage();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void ComboBox()
        {
            OrderOptions = new ObservableCollection<string>
        {
            "2Д707 АС9/ЭП",
            "2Д803 АС9/ЭП",
            "2Т629 АМ-2/ЭП",
            "2Т679 А-2/ЭП",
            "2Т679 Б-2/ЭП",
            "2C569 A-2/ЭП"

        };
            comboBox.ItemsSource = OrderOptions;
        }

        private void CancelOrderButton(object sender, RoutedEventArgs e)
        {
            if (TowarListView.SelectedItem is application selectedApplication)
            {

                if (MessageBox.Show("Вы уверены, что хотите отменить заказ?", "Подтверждение отмены", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
    }
    public class application
    {
        public string Telephone { get; set; }
        public string DateAdd { get; set; }
        public string equipment { get; set; }
        public string Count { get; set; }
        public string UsersFIO { get; set; }
        public string Status { get; set; }

    }
}
