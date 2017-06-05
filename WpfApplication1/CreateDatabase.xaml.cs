using MySql.Data.MySqlClient;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for CreateDatabase.xaml
    /// </summary>
    public partial class CreateDatabase : Window
    {
        MySqlConnection connect;

        public CreateDatabase(MySqlConnection connect)
        {
            this.connect = connect;
            InitializeComponent();
        }

        private void createDbButton_Click(object sender, RoutedEventArgs e)
        {
            createDb();
        }

        private void createDb()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = ("create database " + inputDbName + ";");
                cmd.ExecuteNonQuery();

                useDatabase();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void useDatabase()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = ("Use database " + inputDbName + ";");
                cmd.ExecuteNonQuery();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
