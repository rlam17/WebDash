using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for PrimaryDisplay.xaml
    /// </summary>
    public partial class PrimaryDisplay : Window
    {

        MySqlConnection connect;
        ObservableCollection<ServiceStatus> services;
        List<DateTime> activeDays;

        public PrimaryDisplay(MySqlConnection cn, List<DateTime> ad)
        {
            connect = cn;
            activeDays = ad;
            InitializeComponent();
            enableDates();
            populateList();
        }

        private void populateList()
        {
            //serverCombo.DataContext = null;


            string query = @"SELECT table_schema `Database` FROM INFORMATION_SCHEMA.TABLES WHERE table_name='csv_service';";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "LoadDataBinding");

            MySqlDataReader dr = cmd.ExecuteReader();

            services = new ObservableCollection<ServiceStatus>();

            while (dr.Read())
            {
                services.Add(findFalse(dr[0].ToString()));
            }
            dr.Close();

            //Console.Write(ds);
            //serverCombo.DataContext = ds;
            //serverCombo.DisplayMemberPath = "Database";

            databaseList.ItemsSource = services;

            //serverCombo.ItemsSource = services;
            //databaseList.DataContext = ds;
            //databaseList.DisplayMemberPath = "Database";

            //colorList();

        }

        private void enableDates()
        {
            oCal.SelectionMode = CalendarSelectionMode.SingleDate;
            //oCal.BlackoutDates.Add(new CalendarDateRange(new DateTime(1900, 6, 6), new DateTime(2018, 10, 10)));
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
            //dr.Read();
            int intYear = DateTime.Now.Year;
            DateTime dBegin = new DateTime(intYear, 1, 1);
            DateTime dEnd = new DateTime(intYear, 12, 31);

            bool blnContinue = true;
            while (blnContinue)
            {
                if (dBegin == dEnd)
                {
                    blnContinue = false;
                }

                if (!lAcceptableDates.Contains(dBegin))
                {
                    oCal.BlackoutDates.Add(new CalendarDateRange(dBegin));
                }
                dBegin = dBegin.AddDays(1);
            }


            
            //serverCombo.Text;

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
                return new ServiceStatus(dbName, true);
            }
            catch (Exception )
            {

                return new ServiceStatus(dbName, false);
            }
        }

        private ServiceStatus findFalse(string dbName, DateTime atDay)
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
                return new ServiceStatus(dbName, true);
            }
            catch (Exception)
            {

                return new ServiceStatus(dbName, false);
            }
        }

        private void viewServerButton_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new SubWindow(connect, databaseList.SelectedItem.ToString());
            win2.Show();
        }

        private void createDbButton_Click(object sender, RoutedEventArgs e)
        {
            Window createDbWindow = new CreateDatabase(connect);
            createDbWindow.ShowDialog();
        }

        private void databaseList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            viewServerButton.IsEnabled = true;
        }

        private void disconnectAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }


        private void calendarDayButton_Click(object sender, EventArgs e)
        {
            var bButton = sender as System.Windows.Controls.Primitives.CalendarDayButton;

            // ... See if a date is selected.
            if (bButton.IsBlackedOut)
            {
                Console.WriteLine("Nothing to see here");
                
            }else
            {
                DateTime dDate = (DateTime)bButton.DataContext;
                Console.WriteLine("Does this get poked?");
                Console.WriteLine(dDate);
            }
            
        }

        private void calendarDayButton_Loaded(object sender, EventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            highlightDay(button, date);
            button.DataContextChanged += new DependencyPropertyChangedEventHandler(calendarButton_DataContextChanged);
        }

        private void calendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            highlightDay(button, date);
        }

        private void highlightDay(CalendarDayButton button, DateTime date)
        {
            //if (date == DateTime.Today)
            //{
            //    button.Background = Brushes.SandyBrown;
            //}

            if (activeDays.Contains(date))
            {             
                   
                foreach(ServiceStatus db in services)
                {
                    
                    string dbName = db.ToString();
                    ServiceStatus hasFalse = findFalse(dbName, date);
                    if (!hasFalse.getStatus())
                    {
                        button.Background = Brushes.Red;
                        break;
                    }else
                    {
                        //button.Background = Brushes.SandyBrown;
                        button.Background = Brushes.Green;
                    }
                }

                
            }else
            {
                button.Background = Brushes.LightGray;
            }
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
