using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection connect;
        public MainWindow()
        {            
            InitializeComponent();
            
            Ping heartbeat = new Ping();
            IPAddress sqlAddress = IPAddress.Parse("192.168.7.24");
            PingReply response = heartbeat.Send(sqlAddress);
            if(response.Status == IPStatus.Success)
            {
                sqlStatus.Content = "Online";
                sqlStatusLight.Fill = new SolidColorBrush(Colors.Green);
            } else
            {
                sqlStatus.Content = "Offline";
                sqlStatusLight.Fill = new SolidColorBrush(Colors.Red);
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            //string[] sqlInfo = getSql();
            builder.Add("Server", "192.168.7.24");
            builder.Add("Port", "3306");
            builder.Add("Uid", usernameInput.Text);
            builder.Add("Pwd", passwordInput.Password);
            builder.Add("Database", "server_programs");
            //Console.WriteLine(builder.ConnectionString);

            connect = new MySqlConnection();
            connect.ConnectionString = builder.ConnectionString;

            try
            {
                connect.Open();
                //Program.writeLog("Connection to SQL success");
                serverLabel.Visibility = Visibility.Visible;
                serverCombo.Visibility = Visibility.Visible;
                viewServerButton.Visibility = Visibility.Visible;
                disconnectButton.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Hidden;

                usernameInput.IsEnabled = false;
                passwordInput.IsEnabled = false;

            }
            catch (Exception ex)
            {
                //Program.writeLog("Connection to SQL failed: " + e.Message);
                //Program.exit(2);
            }

            
            
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void viewServerButton_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new SubWindow(connect);
            win2.Show();
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            connect.Close();
            serverLabel.Visibility = Visibility.Hidden;
            serverCombo.Visibility = Visibility.Hidden;
            viewServerButton.Visibility = Visibility.Hidden;
            disconnectButton.Visibility = Visibility.Hidden;
            connectButton.Visibility = Visibility.Visible;

            usernameInput.Clear();
            passwordInput.Clear();

            usernameInput.IsEnabled = true;
            passwordInput.IsEnabled = true;
        }
    }


}
