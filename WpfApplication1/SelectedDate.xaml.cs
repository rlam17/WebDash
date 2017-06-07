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
    /// Interaction logic for SelectedDate.xaml
    /// </summary>
    public partial class SelectedDate : Window
    {
        MySqlConnection connect;
        DateTime focusedDate;

        public SelectedDate(MySqlConnection i, DateTime dt)
        {
            connect = i;
            focusedDate = dt;
            InitializeComponent();
        }
    }
}
