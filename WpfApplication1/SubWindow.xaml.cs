﻿using System;
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

        public SubWindow()
        {
            InitializeComponent();


        }

        public SubWindow(MySqlConnection connect)
        {
            this.connect = connect;
            InitializeComponent();

            string query = @"Select t1.* from server_programs.csv_service t1 inner join (select max(csv_timestmp) recent from server_programs.csv_service) t2 on t1.csv_timestmp = t2.recent;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "LoadDataBinding");
            dataGridResult.DataContext = ds;
            loadRecentReboot();
        }

        private void loadRecentReboot()
        {
            string query = "SELECT conf_settings From server_programs.configfile_info where conf_tagline=\"[configured reboot times]\" order by conf_timestmp DESC limit 1;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            string result = cmd.ExecuteScalar().ToString();
            string[] split = result.Split('\n');
            string restartDate = split[0].Split('=')[1];
            string[] rawInterval = split[1].Split('=')[1].Split(',');

            string period = getPeriod(rawInterval[1]);
            LastReboot.Content = restartDate + "    Interval: " + rawInterval[0] + " " + period;
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

                MessageBoxResult warning = MessageBox.Show("Invalid date or interval!");
            }
        }

        private void updateConfiguredReboot(DateTime date, int intervalNum, char period)
        {
            
            MySqlDataReader recentRow = pullRow();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connect;



            MySqlCommandBuilder builder = new MySqlCommandBuilder();
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
    }
}
