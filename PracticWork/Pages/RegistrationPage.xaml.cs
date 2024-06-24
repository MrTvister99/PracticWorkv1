using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
   
    public partial class RegistrationPage : System.Windows.Controls.Page
    {
        string server = "DESKTOP-9MU0DUB";
        string database = "practicWork";
        string username = "MrTv";
        string passwordDB = "1";
        public bool found;


        public string login;
        public string password;
        public static string FIO1;
        private string roleClient;
        public static string loginUser;
        public RegistrationPage()
        {
            InitializeComponent();
            Application.Current.MainWindow.Title = "Регистрация";
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            podkl();
        }
        private void podkl()
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                login = email.Text;
                password = Password.Password.ToString();
                connection.Open();
                string sql = $"INSERT INTO clients (email,password,Role_client)  VALUES (@email,@password,@Role_client)";
                if (Password.Password.ToString() != "" && email.Text != "")
                {
                    if (!(password.Length < 6) && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]") && Regex.IsMatch(password, "[!@#$%^]"))
                    {
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            loginUser = email.Text;
                            command.Parameters.AddWithValue("@email", $"{login}");

                            command.Parameters.AddWithValue("@password", $"{password}");

                            command.Parameters.AddWithValue("@Role_client", $"1");
                            login = email.Text;
                            NavigationService.Navigate(new UserPage());

                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Пароль должен содержать не менее 6 символов, 1 прописную букву, 1 цифру и 1 символ из набора !@#$%^" +
                            $"");
                    }
                }
                else
                {
                    MessageBox.Show($"Поля не могут быть пустыми");

                }
            }

        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.NavigateToLoginPage();
        }
    }
}
