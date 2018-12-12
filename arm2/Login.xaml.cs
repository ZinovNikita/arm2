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
using System.Windows.Shapes;

namespace arm2
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private MainWindow mainWindow;
        private bool Access = false;
        public Window1()
        {
            InitializeComponent();
        }
        public void CheckLogin(MainWindow mw)
        {
            mainWindow = mw;
            mainWindow.IsEnabled = false;
            this.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User usr = new User();
            User[] arr = usr.Where(login: textBox.Text, password: passwordBox.Password);
            if (arr.Length > 0)
            {
                Access = true;
                mainWindow.CanAccess(arr[0].ID);
                this.Close();
            }
            else
            {
                MessageBox.Show("Не верный логин или пароль");
            }
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Access)
                mainWindow.Close();
        }
    }
}
