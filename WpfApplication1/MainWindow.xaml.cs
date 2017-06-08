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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Globalization;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection connect;
        Timer pingTimer;
        string serverIp = "192.168.7.24";
        string serverPort = "3306";
        ObservableCollection<ServiceStatus> services;
        

        public MainWindow()
        {
            InitializeComponent();

            pingTimer = new Timer();
            pingTimer.Tick += new EventHandler(pingCheck);
            pingTimer.Interval = 2000; // in miliseconds
            pingTimer.Start();

  
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            //string[] sqlInfo = getSql();
            builder.Add("Server", serverIp);
            builder.Add("Port", serverPort);
            builder.Add("Uid", usernameInput.Text);
            builder.Add("Pwd", passwordInput.Password);
            //builder.Add("Database", serverDatabase);
            //Console.WriteLine(builder.ConnectionString);

            connect = new MySqlConnection();
            connect.ConnectionString = builder.ConnectionString;

            try
            {
                connect.Open();
                //Program.writeLog("Connection to SQL success");
                //serverLabel.Visibility = Visibility.Visible;
                //serverCombo.Visibility = Visibility.Visible;
                //viewServerButton.Visibility = Visibility.Visible;
                //disconnectAndExitButton.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Hidden;
                //databaseLabel.Visibility = Visibility.Visible;
                //createDbButton.Visibility = Visibility.Visible;

                usernameInput.IsEnabled = false;
                passwordInput.IsEnabled = false;

                //populateList();

                List<DateTime> passDates = compileDates();

                PrimaryDisplay display = new PrimaryDisplay(connect, passDates);
                display.ShowDialog();


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Connection failed: " + ex.Message);
            }

            // enableDates();

            
        }

        
        private List<DateTime> compileDates()
        {
            List<DateTime> lAcceptableDates = new List<DateTime>();

            string strQuery = @"SELECT EXTRACT(year from log_timestmp) as y, EXTRACT(month from log_timestmp) as m, EXTRACT(day from log_timestmp) as d FROM server_programs.config_log group by y, m, d;";
            MySqlCommand cmd = new MySqlCommand(strQuery, connect);
            MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string composedDate = dr[1].ToString() + "/" + dr[2].ToString() + "/" + dr[0].ToString();

                DateTime date = DateTime.Parse(composedDate, CultureInfo.CreateSpecificCulture("en-US"));
                //Console.WriteLine(date);
                lAcceptableDates.Add(date);
            }
            dr.Close();
            return lAcceptableDates;
        }
       

        

        

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

       

        
        private void pingCheck(object sender, EventArgs e)
        {
            try
            {
                Ping heartbeat = new Ping();
                IPAddress sqlAddress = IPAddress.Parse(serverIp);
                PingReply response = heartbeat.Send(sqlAddress);
                if (response.Status == IPStatus.Success)
                {
                    sqlStatus.Content = "Online";
                    sqlStatusLight.Fill = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    sqlStatus.Content = "Offline";
                    sqlStatusLight.Fill = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception )
            {
                sqlStatus.Content = "Unknown";
                sqlStatusLight.Fill = new SolidColorBrush(Colors.Gray);
            }

        }

        


       

        
    }

    

    
}
