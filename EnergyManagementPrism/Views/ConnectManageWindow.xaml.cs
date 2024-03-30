using EnergyManagementPrism.PublicClass;
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
using System.IO.Ports;
using MySql.Data.MySqlClient;
using System.Data;

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// ConnectManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectManageWindow : Window
    {
        MySQLHelper mySQLHelper = new MySQLHelper();

        public ConnectManageWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmb_com.Items.Add(port);
            }
            DataTable dataTable = mySQLHelper.GetAllConnectData();
            if (dataTable.Rows.Count > 0)
            {
                txt_ip.Text = dataTable.Rows[0]["IP"].ToString();
                txt_port.Text = dataTable.Rows[0]["Port"].ToString();
                cmb_com.Text = dataTable.Rows[0]["Com"].ToString();
                txt_baudrate.Text = dataTable.Rows[0]["BaudRate"].ToString();
                txt_databits.Text = dataTable.Rows[0]["DataBits"].ToString();
                txt_stopbits.Text = dataTable.Rows[0]["StopBits"].ToString();
                cmb_parity.Text = dataTable.Rows[0]["Parity"].ToString();
            }

        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mySQLHelper.UpdateConnectData(txt_ip.Text, txt_port.Text, cmb_com.Text,
                 Convert.ToInt32(txt_baudrate.Text), Convert.ToInt32(txt_databits.Text),
                 Convert.ToInt32(txt_stopbits.Text), cmb_parity.Text);
                HandyControl.Controls.MessageBox.Info("保存成功！", "提示");
            }
            catch
            {
                HandyControl.Controls.MessageBox.Error("保存失败！", "提示");
            }
        }
    }
}

