using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SelectedDate.xaml
    /// </summary>
    public partial class SelectedDate : Window
    {
        MySqlConnection connect;
        DateTime focusedDate;
        ObservableCollection<ServiceStatus> databaseList;

        public SelectedDate(MySqlConnection i, DateTime dt, ObservableCollection<ServiceStatus> dbl)
        {
            connect = i;
            focusedDate = dt;
            databaseList = dbl;
            InitializeComponent();

            display();
        }

        private void display()
        {
            //throw new NotImplementedException();

            dateTitle.Content = focusedDate.ToString("yyyy/MM/dd");
            populateList();
        }

        private void populateList()
        {
            //SELECT * from server_programs.csv_service WHERE CAST(csv_startup as DATE) = '2017-06-01';
            foreach (ServiceStatus item in databaseList)
            {
                Console.WriteLine(item);
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
