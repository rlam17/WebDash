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
        {//phase 1
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = ("create database " + inputDbName.Text + ";");
                cmd.ExecuteNonQuery();

                useDatabase();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void useDatabase()
        {//phase 2
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = ("Use " + inputDbName.Text + ";");
                cmd.ExecuteNonQuery();
                createTables();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void createTables()
        {//phase 3
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = @"CREATE TABLE `csv_service` (
                      `csv_id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '',
                      `csv_startup` DATETIME NOT NULL COMMENT '',
                      `csv_server` VARCHAR(100) NOT NULL COMMENT '',
                      `csv_status` VARCHAR(10) NOT NULL COMMENT '',
                      `csv_service` VARCHAR(100) NOT NULL COMMENT '',
                      `csv_subservice` VARCHAR(100) NULL DEFAULT NULL COMMENT '',
                      `csv_error` VARCHAR(300) NULL DEFAULT NULL COMMENT '',
                      `csv_timestmp` DATETIME NOT NULL COMMENT '',
                      PRIMARY KEY (`csv_id`)  COMMENT '')
                    ENGINE = InnoDB
                    DEFAULT CHARACTER SET = utf8;
                    ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE `configfile_info` (
                      `conf_id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '',
                      `conf_uldate` DATE NOT NULL COMMENT '',
                      `conf_md5hash` VARCHAR(300) NOT NULL COMMENT '',
                      `conf_tagline` VARCHAR(50) NOT NULL COMMENT '',
                      `conf_settings` TEXT NOT NULL COMMENT '',
                      `conf_timestmp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '',
                      PRIMARY KEY (`conf_id`)  COMMENT '')
                    ENGINE = InnoDB
                    DEFAULT CHARACTER SET = utf8;
                    ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE `config_log` (
                      `log_id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '',
                      `log_device` VARCHAR(50) NOT NULL COMMENT '',
                      `log_timestmp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '',
                      PRIMARY KEY (`log_id`)  COMMENT '')
                    ENGINE = InnoDB
                    DEFAULT CHARACTER SET = utf8;
                    ";
                createUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void createUser()
        { //Phase 4
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connect;
                cmd.CommandText = (@"GRANT INSERT, SELECT ON dbTest.* To '" +inputUsername.Text+"'@'hostname' IDENTIFIED BY '"+inputPassword.Password+"';");
                cmd.ExecuteNonQuery();
                MessageBox.Show("Database created!");
                Close();
            }
            catch (Exception ex)
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
