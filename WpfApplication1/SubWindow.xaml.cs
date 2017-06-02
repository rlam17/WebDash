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

        }

    }
}
