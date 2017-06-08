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
using MySql.Data.MySqlClient;
using System.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        private MySqlConnection connect;
        private char selectRadio;
        private string database;
        private DateTime date;

        public SubWindow()
        {
            InitializeComponent();


        }

        public SubWindow(MySqlConnection connection, string db)
        {
            
            connect = connection;
            database = db;
            InitializeComponent();

            string query = @"Select t1.* from "+database+".csv_service t1 inner join (select max(csv_timestmp) recent from " + database +".csv_service) t2 on t1.csv_timestmp = t2.recent;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "LoadDataBinding");
            dataGridResult.DataContext = ds;
            title.Content = "Database: " + db;
            loadRecentReboot();
        }

        public SubWindow(MySqlConnection connection, string db, DateTime dt)
        {

            connect = connection;
            database = db;
            InitializeComponent();
            date = dt;
            string strDate = dt.ToString("yyyy-MM-dd");

            string query = @"Select * from " + database + ".csv_service Where " + strDate + ";";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "LoadDataBinding");
            dataGridResult.DataContext = ds;
            title.Content = "Database: " + db;
            loadRecentReboot();
        }

        private void loadRecentReboot()
        {
            try
            {
                string query = "SELECT conf_settings From " + database + ".configfile_info where conf_tagline=\"[configured reboot times]\" order by conf_timestmp DESC limit 1;";
                MySqlCommand cmd = new MySqlCommand(query, connect);
                string result = cmd.ExecuteScalar().ToString();
                string[] split = result.Split('\n');
                string restartDate = split[0].Split('=')[1];
                string[] rawInterval = split[1].Split('=')[1].Split(',');

                string period = getPeriod(rawInterval[1]);
                LastReboot.Content = restartDate + "    Interval: " + rawInterval[0] + " " + period;
            }
            catch (Exception)
            {
                MessageBox.Show("This server does not have a reboot yet");
                this.Close();
            }
            
        }
        private void changeColours()
        {

        }
        private string getPeriod(string input)
        {
            if(input.ToLower() == "s")
            {
                return "seconds";
            }
            else if(input.ToLower() == "m") {

                return "minutes";
            } else if(input.ToLower() == "h")
            {
                return "hours";
            }else if(input.ToLower() == "d"){
                return "days";
            }else if(input.ToLower() == "w")
            {
                return "weeks";
            } else if(input.ToLower() == "m")
            {
                return "months";
            }
            else
            {
                return "";
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime newStartDate = (DateTime)dtpStartTime.Value;
                int interval = (int)intervalAmount.Value;                
                updateConfiguredReboot(newStartDate, interval, selectRadio);
            } catch (Exception ex)
            {
                throw (ex);
                MessageBoxResult warning = MessageBox.Show("Invalid date or interval!");
            }
        }

        private void updateConfiguredReboot(DateTime date, int intervalNum, char period)
        {
            
            MySqlDataReader recentRow = pullRow();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connect;
            recentRow.Read();

            string newRebootTime;

            newRebootTime = "Start=" + date + "\n" + "Interval=" + intervalNum + "," + period + "\n";

            
            try
            {
                cmd.CommandText = "INSERT INTO " +
                    "configfile_info(conf_id, conf_uldate, conf_md5hash, conf_tagline, conf_settings, conf_timestmp) " +
                    "VALUES(@conf_id, @conf_uldate, @conf_md5hash, @conf_tagline, @conf_settings, @conf_timestmp)";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@conf_id", null);
                cmd.Parameters.AddWithValue("@conf_uldate", Convert.ToDateTime(recentRow[1]));
                cmd.Parameters.AddWithValue("@conf_md5hash", recentRow[2]);
                cmd.Parameters.AddWithValue("@conf_tagline", @"[configured reboot times]");
                cmd.Parameters.AddWithValue("@conf_settings", newRebootTime);
                cmd.Parameters.AddWithValue("@conf_timestmp", DateTime.Now);

                recentRow.Close();

                cmd.ExecuteNonQuery();

                MessageBox.Show("Reboot time has been successfully configured.");
            }catch(Exception ex)
            {
                throw (ex);
                MessageBox.Show(ex.Message);
            }


            
        }

        private MySqlDataReader pullRow()
        {
            //Pull row with: SELECT * from server_programs.configfile_info ORDER BY conf_id DESC LIMIT 1;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connect;
            cmd.CommandText = @"SELECT * from server_programs.configfile_info ORDER BY conf_id DESC LIMIT 1;";
            
            return cmd.ExecuteReader();

        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            if (ck.IsChecked.Value)
            {
                selectRadio = ck.Content.ToString()[0];
            }
        }

        private void dataGridResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView rowView = dataGrid.SelectedItem as DataRowView;
            string strSelected = "";
            try{
                strSelected = rowView.Row[5].ToString();
            }
            catch (Exception ex) { throw (ex); }
            

            if (!String.IsNullOrEmpty(strSelected))
            {
                string query = "Select * from server_programs.csv_service t1 inner join (select max(csv_timestmp) recent from server_programs.csv_service) t2 on t1.csv_timestmp = t2.recent where csv_subservice =\"" + strSelected + "\";";
                MySqlCommand sqlQuery = new MySqlCommand(query, connect);
                MySqlDataAdapter dAdaptor = new MySqlDataAdapter(sqlQuery);
                DataSet dSet = new DataSet();
                dAdaptor.Fill(dSet, "subDataBind");
                subGrid.DataContext = dSet;
                sqlLabel.Content = "Subservice: "+strSelected;
            }
            else
            {
                string query = "select 1 from dual where false;";
                MySqlCommand sqlQuery = new MySqlCommand(query, connect);
                MySqlDataAdapter dAdaptor = new MySqlDataAdapter(sqlQuery);
                DataSet dSet = new DataSet();
                dAdaptor.Fill(dSet, "subDataBind");
                subGrid.DataContext = dSet;
                sqlLabel.Content = "Subservice";
            }

            

        }
    }
}
