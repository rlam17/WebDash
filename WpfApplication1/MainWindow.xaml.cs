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
                serverLabel.Visibility = Visibility.Visible;
                serverCombo.Visibility = Visibility.Visible;
                viewServerButton.Visibility = Visibility.Visible;
                disconnectButton.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Hidden;
                databaseLabel.Visibility = Visibility.Visible;
                createDbButton.Visibility = Visibility.Visible;

                usernameInput.IsEnabled = false;
                passwordInput.IsEnabled = false;

                populateCombo();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Connection failed: " + ex.Message);
            }

            blackDates();
        }

        private void blackDates()
        {
            oCal.BlackoutDates.Add(new CalendarDateRange(new DateTime(2017, 7, 8), new DateTime(2017, 7, 11)));
        }

        private void populateCombo()
        {
            serverCombo.DataContext = null;

            string query = @"SHOW Databases;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "LoadDataBinding");
           
            //Console.Write(ds);
            serverCombo.DataContext = ds;
            serverCombo.DisplayMemberPath = "Database";
            databaseList.DataContext = ds;
            databaseList.DisplayMemberPath = "Database";

            colorList();
            
        }

        private void colorList()
        {
            foreach(DataRowView item in databaseList.Items)
            {
                Console.WriteLine(item.Row[0].ToString());
                try
                {
                    string dbName = item.Row[0].ToString();
                    bool isThereAFalse = findFalse(dbName);
                    
                } catch(Exception ex)
                {

                }
            }
        }

        private bool findFalse(string dbName)
        {
            string query = @"Select t1.* from " + dbName + ".csv_service t1 inner join (select max(csv_timestmp) recent from " + dbName + ".csv_service) t2 on t1.csv_timestmp = t2.recent;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            return false;
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void viewServerButton_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new SubWindow(connect, serverCombo.Text);
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
            databaseLabel.Visibility = Visibility.Hidden;
            createDbButton.Visibility = Visibility.Hidden;
            viewServerButton.Visibility = Visibility.Hidden;

            

            usernameInput.Clear();
            passwordInput.Clear();

            usernameInput.IsEnabled = true;
            passwordInput.IsEnabled = true;
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
            catch (Exception ex)
            {
                sqlStatus.Content = "Unknown";
                sqlStatusLight.Fill = new SolidColorBrush(Colors.Gray);
            }

        }

        private void serverCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            viewServerButton.IsEnabled = true;
        }

        private void createDbButton_Click(object sender, RoutedEventArgs e)
        {
            Window createDbWindow = new CreateDatabase(connect);
            createDbWindow.ShowDialog();
        }

        private void databaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    
}
