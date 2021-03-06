﻿using MySql.Data.MySqlClient;
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

        public static MySqlConnection connect;
        ObservableCollection<ServiceStatus> services;
        List<DateTime> activeDays;

        public PrimaryDisplay(MySqlConnection cn, List<DateTime> ad)
        {
            connect = cn;
            activeDays = ad;
            InitializeComponent();
            enableDates();
            populateList();

            oCal.DisplayDateStart = new DateTime(DateTime.Now.Year, 1, 1);
            oCal.DisplayDateEnd = new DateTime(DateTime.Now.Year, 12, 31);
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

            List<string> databases = new List<string>();

            while (dr.Read())
            {
                databases.Add((string)dr[0]);
            }
            dr.Close();

            foreach(string item in databases)
            {
                services.Add(findFalse(item));
            }

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
                MySqlConnection con = connect;
                string query = @"Select t1.* from " + dbName + ".csv_service t1 inner join (select max(csv_timestmp) recent from " + dbName + ".csv_service) t2 on t1.csv_timestmp = t2.recent WHERE csv_status = 'false';";
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader dr = cmd.ExecuteReader();
                //int rowNumber = 0;

                if (dr.Read())
                {
                    dr.Close();
                    return new ServiceStatus(dbName, 0);
                }else
                {
                    query = @"Select t1.* from " + dbName + ".csv_service t1 inner join (select max(csv_timestmp) recent from " + dbName + ".csv_service) t2 on t1.csv_timestmp = t2.recent WHERE csv_status = 'true';";
                    dr.Close();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        return new ServiceStatus(dbName, 1);
                    }
                    else
                    {
                        dr.Close();
                        return new ServiceStatus(dbName, 2);
                    }
                }
                //while (dr.Read())
                //{
                //    rowNumber++;
                //    if (String.Compare(dr[3].ToString(), "false") == 0)
                //    {
                //        dr.Close();
                //        return new ServiceStatus(dbName, 0);
                //    }
                //}
                //dr.Close();
                //if(rowNumber > 0)
                //{
                //    return new ServiceStatus(dbName, 1);
                //}else
                //{
                //    return new ServiceStatus(dbName, 2);
                //}
                
            }
            catch (Exception )
            {

                return new ServiceStatus(dbName, 2);
            }
        }

        private ServiceStatus findFalse(string dbName, DateTime atDay)
        {
            string compose = atDay.Year.ToString() + "-" + atDay.Month.ToString() + "-" + atDay.Day.ToString();
            try
            {
                //SELECT* from server_programs.csv_service WHERE CAST(csv_startup as DATE) = '2017-06-01';
                string query = @"SELECT * from "+dbName+".csv_service WHERE CAST(csv_startup as DATE) = '"+compose+"' AND csv_status='false';";
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataReader dr = cmd.ExecuteReader();

                //while (dr.Read())
                //{
                //    if (String.Compare(dr[3].ToString(), "false") == 0)
                //    {
                //        dr.Close();
                //        return new ServiceStatus(dbName, 0);
                //    }
                //}
                //dr.Close();
                //return new ServiceStatus(dbName, 1);
                if (dr.Read())
                {
                    dr.Close();
                    return new ServiceStatus(dbName, 0);
                }
                else
                {
                    query = @"SELECT * from " + dbName + ".csv_service WHERE CAST(csv_startup as DATE) = '" + compose + "' AND csv_status='true';";
                    dr.Close();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        return new ServiceStatus(dbName, 1);
                    }
                    else
                    {
                        dr.Close();
                        return new ServiceStatus(dbName, 2);
                    }
                }
            }
            catch (Exception)
            {

                return new ServiceStatus(dbName, 2);
            }
        }

        private void viewServerButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceStatus item = (ServiceStatus)databaseList.SelectedItem;

            Window win2 = new SubWindow(connect, item);
            try
            {
                
                win2.Show();
            }
            catch (Exception)
            {
                win2.Close();
            }
            
        }

        private void createDbButton_Click(object sender, RoutedEventArgs e)
        {
            Window createDbWindow = new CreateDatabase(connect);
            createDbWindow.ShowDialog();
        }

        private void databaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewServerButton.IsEnabled = true;
        }

        private void disconnectAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
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
                Window selectDate = new SelectedDate(connect, dDate, services);
                selectDate.ShowDialog();
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
            //bool proceed = true;
            if (activeDays.Contains(date))
            {             
                
                foreach(ServiceStatus db in services)
                {
                    string dbName = db.getOriginalName();
                    ServiceStatus hasFalse = findFalse(dbName, date);

                    if (hasFalse.getStatus() == 0)
                    {
                        button.Background = Brushes.Red;
                        break;
                    }
                    else
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

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void notificationButton_Click(object sender, RoutedEventArgs e)
        {
            Window email = new EmailWindow();
            email.ShowDialog();
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
        private string _originalName;
        public string originalName
        {
            get
            {
                return _originalName;
            }set
            {
                _originalName = value;
            }
        }
        
        private int _status;
        public int status
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

        public ServiceStatus(string inputName, int inputStatus)
        {
            
            _originalName = inputName;
            _name = generateName();
            _status = inputStatus;
        }

        public ServiceStatus(ServiceStatus copy)
        {
            _name = copy._name;
            _originalName = copy._originalName;
            _status = copy._status;
        }

        public string generateName()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = PrimaryDisplay.connect;
                string query = @"SELECT dbal_dbalias from server_programs.dbase_alias WHERE dbal_dbname = '" + _originalName + "';";
                cmd.CommandText = query;
                string result = cmd.ExecuteScalar().ToString();

                return result;
            }
            catch (Exception)
            {
                return _originalName;
            }
        }
        
        public override string ToString()
        {
            return _name;
            
        }

        public int getStatus()
        {
            return _status;
        }

        public string getOriginalName()
        {
            return _originalName;
        }
    }
}
