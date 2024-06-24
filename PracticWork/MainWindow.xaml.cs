using PracticWork.Pages;
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

namespace PracticWork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public  partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new LoginPage());
        }
        public void NavigateToLoginPage()
        {
            frame.NavigationService.Navigate(new LoginPage());
        }
        public void NavigateToRegistritionPage()
        {
            frame.NavigationService.Navigate(new RegistrationPage());
        }
    }
}
