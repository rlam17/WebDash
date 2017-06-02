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
            string query = "Select t1.conf_settings from server_programs.configfile_info t1 inner join (select max(conf_timestmp) recent from server_programs.configfile_info) t2 on t1.conf_timestmp = t2.recent where t1.conf_tagline=\"[configured reboot times]\" LIMIT 1;";
            MySqlCommand cmd = new MySqlCommand(query, connect);
            string result = cmd.ExecuteScalar().ToString();
            string[] split = result.Split('\n');
            string restartDate = split[0].Split('=')[1];
            LastReboot.Content = restartDate;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime newStartDate = (DateTime)dtpStartTime.Value;
                int interval = (int)intervalAmount.Value;
            } catch (Exception ex)
            {
                
                MessageBoxResult warning = MessageBox.Show("Invalid start date or interval!");
            }            
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
