﻿using MySql.Data.MySqlClient;
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
                cmd.CommandText = ("create database " + inputDbName + ";");
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
                cmd.CommandText = ("Use database " + inputDbName + ";");
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
                cmd.CommandText = @"CREATE TABLE `test`.`csv_service` (
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
                cmd.CommandText = "";
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
                cmd.CommandText = (@"GRANT INSERT, SELECT ON dbTest.* To 'user'@'hostname' IDENTIFIED BY 'password';");
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
