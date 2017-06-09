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

            dateTitle.Content = "Date: " + focusedDate.ToString("dd/MM/yyyy");
            populateList();
        }

        private void populateList()
        {
            
            ObservableCollection<ServiceStatus> serversAtDate = new ObservableCollection<ServiceStatus>();
            foreach (ServiceStatus item in databaseList)
            {
                serversAtDate.Add(findError(item));
            }

            databaseListbox.ItemsSource = serversAtDate;
        }

        private ServiceStatus findError(ServiceStatus i)
        {
            //SELECT * from server_programs.csv_service WHERE CAST(csv_startup as DATE) = '2017-06-01';
            try
            {
                string atDate = focusedDate.Year.ToString() + "-" + focusedDate.Month.ToString() + "-" + focusedDate.Day.ToString();
                string query = @"SELECT * from "+i.ToString()+".csv_service WHERE CAST(csv_startup as DATE) = '"+atDate+"';";
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (String.Compare(dr[3].ToString(), "false") == 0)
                    {
                        dr.Close();
                        return new ServiceStatus(i.ToString(), 0);
                    }
                }
                dr.Close();
                return new ServiceStatus(i.ToString(), 1);
            }
            catch (Exception )
            {

                return new ServiceStatus(i.ToString(), 2);
            }
        }


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void databaseListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Window win2 = new SubWindow(connect, databaseListbox.SelectedItem.ToString(), focusedDate);
            try
            {

                win2.Show();
            }
            catch (Exception)
            {
                win2.Close();
            }
        }
    }
}
