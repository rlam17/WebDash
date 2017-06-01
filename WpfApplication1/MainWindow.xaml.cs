using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
                sqlStatusLight.Fill = "#00ff1e";
            } else
            {
                sqlStatus.Content = "Offline";
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            serverLabel.Visibility = Visibility.Visible;
            serverCombo.Visibility = Visibility.Visible;
            viewServerButton.Visibility = Visibility.Visible;
            
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void viewServerButton_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new SubWindow();
            win2.Show();
        }
    }


}
