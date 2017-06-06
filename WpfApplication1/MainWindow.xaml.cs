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

using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
                disconnectAndExitButton.Visibility = Visibility.Visible;
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

            enableDates();
        }

        private void enableDates()
        {
            oCal.SelectionMode = CalendarSelectionMode.SingleDate;
            oCal.BlackoutDates.Add(new CalendarDateRange(new DateTime(1900, 6, 6), new DateTime(2018, 10, 10)));
            List<DateTime> lAcceptableDates = new List<DateTime>();
            string strQuery = @"SELECT EXTRACT(year from log_timestmp) as y, EXTRACT(month from log_timestmp) as m, EXTRACT(day from log_timestmp) as d FROM server_programs.config_log group by y, m, d;";
            MySqlCommand cmd = new MySqlCommand(strQuery, connect);
            MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string composedDate = dr[1].ToString() + "/" + dr[2].ToString() + "/" + dr[0].ToString();

                DateTime date = DateTime.Parse(composedDate, CultureInfo.CreateSpecificCulture("en-US"));
                Console.WriteLine(date);
                lAcceptableDates.Add(date);
                
                
                
            }
            //dr.Read();


            dr.Close();
            
            foreach(DateTime date in lAcceptableDates)
            {
                if (oCal.BlackoutDates.Contains(date))
                {
                    oCal.BlackoutDates.Remove(new CalendarDateRange(date, date.AddDays(1)));
                }
            }

            

            //serverCombo.Text;

        }

        private void populateCombo()
        {
            serverCombo.DataContext = null;
            

            string query = @"SELECT table_schema `Database` FROM INFORMATION_SCHEMA.TABLES WHERE table_name='csv_service';";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "LoadDataBinding");

            MySqlDataReader dr = cmd.ExecuteReader();

            ObservableCollection<ServiceStatus> services = new ObservableCollection<ServiceStatus>();

            while (dr.Read())
            {
                services.Add(findFalse(dr[0].ToString()));
            }

            //Console.Write(ds);
            //serverCombo.DataContext = ds;
            //serverCombo.DisplayMemberPath = "Database";
            dr.Close();
            databaseList.ItemsSource = services;


            //databaseList.DataContext = ds;
            //databaseList.DisplayMemberPath = "Database";
            
            //colorList();
            
        }

        private void colorList()
        {
            //foreach(DataRowView item in databaseList.Items)
            //{
            //    //Console.WriteLine(item.Row[0].ToString());
            //    if (findFalse(item.Row[0].ToString()) == 1)
            //    {
                    
            //    }
            //}
        }

        private ServiceStatus findFalse(string dbName)
        {
            try
            {
                string query = @"Select t1.* from " + dbName + ".csv_service t1 inner join (select max(csv_timestmp) recent from " + dbName + ".csv_service) t2 on t1.csv_timestmp = t2.recent;";
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (String.Compare(dr[3].ToString(), "false") == 0)
                    {
                        dr.Close();
                        return new ServiceStatus(dbName, false);
                    }
                }
                dr.Close();
                return new ServiceStatus(dbName, false);
            }
            catch (Exception ex)
            {

                return new ServiceStatus(dbName, false);
            }
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


        private void databaseList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void disconnectAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
    
    public class ServiceStatus
    {
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private bool _status;
        public bool status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }

        }

        public ServiceStatus(string inputName, bool inputStatus)
        {
            _name = inputName;
            _status = inputStatus;
        }

        public override string ToString()
        {
            return _name;
        }

        public bool getStatus()
        {
            return _status;
        }
    }

    
}
