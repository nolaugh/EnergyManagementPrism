using EnergyManagementPrism.PublicClass;
using EnergyManagementPrism.ViewModels;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HslCommunication.Profinet.Siemens;
using HslCommunication;
using System.IO.Ports;
using HandyControl.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using LiveChartsCore.Defaults;
using EnergyManagementPrism.Model;
using NCalc;

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// MonitorUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorUserControl : UserControl
    {
        MySQLHelper mySQLHelper = new MySQLHelper();
        private DateTime lastUpdateTime = DateTime.Now;
        private DateTime lastUpdateTime1= DateTime.Now;
        public string ip = null;
        public int port = 0;
        public string com = null;
        public int baudrate = 0;
        public int databits = 0;
        public int stopbits = 0;
        public string parity = null;
        public int parityReslt = 0;
        TcpClient client = new TcpClient();
        NetworkStream stream;

        public MonitorUserControl()
        {
            InitializeComponent(); 

            MonitorUserControlViewModel monitorUserControlViewModel=new MonitorUserControlViewModel();
            this.DataContext = monitorUserControlViewModel;

            chart_Online.Series = new ISeries[] {
                                new PieSeries<int> { Values = new int[] { 0 },Name="Connect",
                                    Fill = new SolidColorPaint(new SKColor(170, 255, 86))},
                                new PieSeries<int> { Values = new int[] { 4},Name="Disconnect",
                                    Fill = new SolidColorPaint(new SKColor(255, 86, 86))},
            };

            chart_state.Series = new ISeries[]
            {
                 new PieSeries<double> { Values = new double[] { 0 },Name="Info", Fill = new SolidColorPaint(new SKColor(86, 170, 255))},
                 new PieSeries<double> { Values = new double[] { 0 },Name="Warn", Fill = new SolidColorPaint(new SKColor(255, 255, 86))},
                new PieSeries<double> { Values = new double[] { 4 },Name="Error",Fill = new SolidColorPaint(new SKColor(255, 86, 86))},
            };

            string queryGetIpPort = "SELECT * FROM tb_connect";
            DataTable dataTableGetIPPort = mySQLHelper.ExecuteQuery(queryGetIpPort);
            foreach (DataRow row in dataTableGetIPPort.Rows)
            {
                ip = row["IP"].ToString();
                port = Convert.ToInt32(row["Port"]);
                com = row["COM"].ToString();
                baudrate = Convert.ToInt32(row["Baudrate"]);
                databits = Convert.ToInt32(row["Databits"]);
                stopbits = Convert.ToInt32(row["Stopbits"]);
                parity = row["Parity"].ToString();
                if (parity == "无") { parityReslt = 0; }
                if (parity == "奇") { parityReslt = 1; }
                if (parity == "偶") { parityReslt = 2; }
            }

            try
            {
                client.ConnectAsync(IPAddress.Parse(ip), port);
               
                stream = client.GetStream();
            }
            catch
            { 
            }
             

            GetDataTask();
            ControlTask();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 获取所有设备信息
                string queryOne = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 1";
                DataTable dataTableOne = mySQLHelper.ExecuteQuery(queryOne);
                foreach (DataRow row in dataTableOne.Rows)
                {
                    txt_SOne.Text = row["Name"].ToString();
                    txt_SOneVo.Text = row["Volotage"].ToString();
                    txt_SOneE.Text = row["Electricit"].ToString();
                    txt_SOneP.Text = row["Power"].ToString();
                }

                string queryTwo = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 3";
                DataTable dataTableTwo = mySQLHelper.ExecuteQuery(queryTwo);
                foreach (DataRow row in dataTableTwo.Rows)
                {
                    txt_STwo.Text = row["Name"].ToString();
                    txt_STwoVo.Text = row["Volotage"].ToString();
                    txt_STwoE.Text = row["Electricit"].ToString();
                    txt_STwoP.Text = row["Power"].ToString();
                }

                string queryThree = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 4";
                DataTable dataTableThree = mySQLHelper.ExecuteQuery(queryThree);
                foreach (DataRow row in dataTableThree.Rows)
                {
                    txt_OCOne.Text = row["Name"].ToString();
                    txt_OCVo.Text = row["Volotage"].ToString();
                    txt_OCE.Text = row["Electricit"].ToString();
                    txt_OCP.Text = row["Power"].ToString();
                }

                string queryFour = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 5";
                DataTable dataTableFour = mySQLHelper.ExecuteQuery(queryFour);
                foreach (DataRow row in dataTableFour.Rows)
                {
                    txt_OCTwo.Text = row["Name"].ToString();
                }

                string queryFive = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 6";
                DataTable dataTableFive = mySQLHelper.ExecuteQuery(queryFive);
                foreach (DataRow row in dataTableFive.Rows)
                {
                    txt_Plc.Text = row["Name"].ToString();
                    txt_PlcV.Text = row["Volotage"].ToString();
                    txt_PlcE.Text = row["Electricit"].ToString();
                    txt_PlcP.Text = row["Power"].ToString();
                }

                #endregion

                #region 获取CartesianChart
                DateTime currentDate = DateTime.Today;

                // 构建查询条件
                StringBuilder queryBuilder = new StringBuilder();
                List<string> devices = new List<string>() { "DeviceOne", "DeviceTwo", "DO", "PLC" };
                List<string> devicesReal = new List<string>() { "一号传感器", "二号传感器", "三号开关量", "四号PLC设备" };
                for (int i = 0; i < devices.Count; i++)
                {
                    queryBuilder.Append("SELECT SUM(UsingPower) AS Sum" + i + " FROM tb_historydata WHERE Name = '" + devicesReal[i] + "' AND Time >= '" + currentDate.ToString("yyyy-MM-dd 00:00:00") + "' AND Time <= '" + currentDate.ToString("yyyy-MM-dd 23:59:59") + "'");
                    if (i < devices.Count - 1)
                    {
                        queryBuilder.Append(" UNION  ALL ");
                    }
                }

                // 执行查询
                DataTable dataTableGetData = mySQLHelper.ExecuteQuery(queryBuilder.ToString());
                var values = dataTableGetData.Rows.Cast<DataRow>()
                 .Select(row =>
                 {
                     string valueStr = row[0].ToString();
                     double value;
                     if (double.TryParse(valueStr, out value))
                     {
                         return value;
                     }
                     else
                     {
                         // 如果无法转换为double，返回一个默认值或者其他适当的处理方式
                         return 0.0; // 返回默认值0.0
                     }
                 })
                 .ToArray();


                chart_Sum.Series = new ISeries[]
                    {
                new ColumnSeries<double>
                { Values = values }
                    };

                chart_Sum.XAxes = new[]
                {
                new Axis
                {
                    Labels = devices.ToArray() ,
                    LabelsPaint = new SolidColorPaint(new SkiaSharp.SKColor(86, 170, 255))
                }
             };

                chart_Sum.YAxes = new[]
                {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(new SkiaSharp.SKColor(86, 170, 255))
                }
            };

                #endregion
            }
            catch
            {
                
            }
        }

        //暂时不用
        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        public async void GetDataTask()
        {
            //数据采集线程
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {

                        #region 实时数据采集

                        #region 指令介绍
                        //温湿度设备 地址0x01 寄存器地址 湿度0x00 温度0x01
                        //请求01 03 00 00 00 02 C4 0B

                        //光照设备 地址0x02 寄存器地址 光照度0x07
                        //请求02 03 00 07 00 01 35 F8

                        //DO设备 地址0x03
                        //DO读取 03 01 00 00 00 04 3C 2B 
                        //返回 03 01 01 00 50 30 全部断开
                        //返回 03 01 01 01 91 F0 DO1打开
                        //返回 03 01 01 02 D1 F1 DO2打开
                        //返回 03 01 01 03 10 31 全部打开
                        //DO1闭合指令  03 05 00 00 FF 00 8D D8 
                        //DO1断开指令  03 05 00 00 00 00 CC 28 
                        //DO2闭合指令  03 05 00 01 FF 00 DC 18 
                        //DO2断开指令  03 05 00 01 00 00 9D E8 

                        //西门子PLC S7-200 PPI协议
                        //寄存器地址 DB1.0等
                        //读取寄存器地址 DB1.0等
                        //吸入寄存器地址 DB1.0等
                        #endregion

                        #region 湿度
                        //湿度查询指令
                        string queryGetAddressOne = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 1";
                        DataTable dataTableGetAddressOne = mySQLHelper.ExecuteQuery(queryGetAddressOne);
                        string addressone = "";
                        foreach (DataRow row in dataTableGetAddressOne.Rows)
                        {
                            addressone = row["Address"].ToString();
                        }
                        byte[] addressBytesOne = Enumerable.Range(0, addressone.Length)
                                             .Where(x => x % 2 == 0)
                                             .Select(x => Convert.ToByte(addressone.Substring(x, 2), 16))
                                             .ToArray();

                        byte[] requestDataone = { 0x01, 0x03, addressBytesOne[0], addressBytesOne[1], 0x00, 0x01 };
                        ushort crcone = CalculateCRC(requestDataone);
                        byte crcLowone = (byte)(crcone & 0x00FF);
                        byte crcHighone = (byte)((crcone >> 8) & 0x00FF);
                        byte[] completeDataOne = requestDataone.Concat(new byte[] { crcLowone, crcHighone }).ToArray();
                        #endregion

                        #region 温度
                        //温度查询指令
                        string queryGetAddressTwo = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 2";
                        DataTable dataTableGetAddressTwo = mySQLHelper.ExecuteQuery(queryGetAddressTwo);
                        string addresstwo = "";
                        foreach (DataRow row in dataTableGetAddressTwo.Rows)
                        {
                            addresstwo = row["Address"].ToString();
                        }
                        byte[] addressBytesTwo = Enumerable.Range(0, addresstwo.Length)
                                             .Where(x => x % 2 == 0)
                                             .Select(x => Convert.ToByte(addresstwo.Substring(x, 2), 16))
                                             .ToArray();

                        byte[] requestDatatwo = { 0x01, 0x03, addressBytesTwo[0], addressBytesTwo[1], 0x00, 0x01 };
                        ushort crctwo = CalculateCRC(requestDatatwo);
                        byte crcLowtwo = (byte)(crctwo & 0x00FF);
                        byte crcHightwo = (byte)((crctwo >> 8) & 0x00FF);
                        byte[] completeDataTwo = requestDatatwo.Concat(new byte[] { crcLowtwo, crcHightwo }).ToArray();
                        #endregion

                        #region 光照度
                        //温度查询指令
                        string queryGetAddressThree = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 3";
                        DataTable dataTableGetAddressThree = mySQLHelper.ExecuteQuery(queryGetAddressThree);
                        string addressthree = "";
                        foreach (DataRow row in dataTableGetAddressThree.Rows)
                        {
                            addressthree = row["Address"].ToString();
                        }
                        byte[] addressBytesThree = Enumerable.Range(0, addressthree.Length)
                                             .Where(x => x % 2 == 0)
                                             .Select(x => Convert.ToByte(addressthree.Substring(x, 2), 16))
                                             .ToArray();

                        byte[] requestDatathree = { 0x02, 0x03, addressBytesThree[0], addressBytesThree[1], 0x00, 0x01 };
                        ushort crcthree = CalculateCRC(requestDatathree);
                        byte crcLowthree = (byte)(crcthree & 0x00FF);
                        byte crcHighthree = (byte)((crcthree >> 8) & 0x00FF);
                        byte[] completeDataThree = requestDatathree.Concat(new byte[] { crcLowthree, crcHighthree }).ToArray();
                        #endregion

                        #region DO1/DO2
                        //读取指令
                        byte[] doReadbytes = { 0x03, 0x01, 0x00, 0x00, 0x00, 0x04, 0x3C, 0x2B };
                        #endregion

                        #region TCP通讯模块

                        stream.Write(completeDataOne, 0, completeDataOne.Length);
                        stream.Flush();
                        Thread.Sleep(200);
                        byte[] bufferone = new byte[1024];
                        int bytesReadone = stream.Read(bufferone, 0, bufferone.Length);
                        byte[] responseDataone = bufferone.Take(bytesReadone).ToArray();
                        string hexStringone = BitConverter.ToString(responseDataone).Replace("-", "");
                        string subStringone = hexStringone.Substring(6, 4);
                        int valueOne = Convert.ToInt32(subStringone, 16);
                        decimal valueOneresult = Convert.ToDecimal(valueOne) / 10m;

                        Thread.Sleep(200);

                        stream.Write(completeDataTwo, 0, completeDataTwo.Length);
                        stream.Flush();
                        Thread.Sleep(200);
                        byte[] buffertwo = new byte[1024];
                        int bytesReadtwo = stream.Read(buffertwo, 0, buffertwo.Length);
                        byte[] responseDatatwo = buffertwo.Take(bytesReadtwo).ToArray();
                        string hexStringtwo = BitConverter.ToString(responseDatatwo).Replace("-", "");
                        string subStringtwo = hexStringtwo.Substring(6, 4);
                        int valueTwo = Convert.ToInt32(subStringtwo, 16);
                        decimal valueTworesult = Convert.ToDecimal(valueTwo) / 10m;

                        Thread.Sleep(200);

                        stream.Write(completeDataThree, 0, completeDataThree.Length);
                        stream.Flush();
                        Thread.Sleep(200);
                        byte[] bufferthree = new byte[1024];
                        int bytesReadthree = stream.Read(bufferthree, 0, bufferthree.Length);

                        byte[] responseDatathree = bufferthree.Take(bytesReadthree).ToArray();
                        string hexStringthree = BitConverter.ToString(responseDatathree).Replace("-", "");
                        string subStringthree = hexStringthree.Substring(6, 4);
                        int valueThree = Convert.ToInt32(subStringthree, 16);
                        decimal valueThreeresult = Convert.ToDecimal(valueThree);

                        Thread.Sleep(200);

                        stream.Write(doReadbytes, 0, doReadbytes.Length);
                        stream.Flush();
                        Thread.Sleep(200);
                        byte[] bufferDo = new byte[1024];
                        int bytesReadDo = stream.Read(bufferDo, 0, bufferDo.Length);
                        byte[] responseDataDo = bufferDo.Take(bytesReadDo).ToArray();
                        string hexStringDo = BitConverter.ToString(responseDataDo).Replace("-", "");
                        string subStringDo = hexStringDo.Substring(6, 2);
                        int valueDo = Convert.ToInt32(subStringDo, 16);
                        decimal valueDoresult = Convert.ToDecimal(valueDo);

                        Thread.Sleep(200);



                        #region 西门子PPI通讯模块                            
                        SiemensPPI siemensPPI = new SiemensPPI();
                        siemensPPI.SerialPortInni(sp =>
                        {
                            sp.PortName = com;
                            sp.BaudRate = baudrate;
                            sp.DataBits = databits;
                            sp.StopBits = stopbits == 0 ? System.IO.Ports.StopBits.None :
                            (stopbits == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two);
                            sp.Parity = parityReslt == 0 ? System.IO.Ports.Parity.None :
                            (parityReslt == 1 ? System.IO.Ports.Parity.Odd : System.IO.Ports.Parity.Even);
                        });
                        siemensPPI.Open();
                        siemensPPI.Station = 2;
                        string queryGetAddressFour = "SELECT d.* FROM tb_device AS d JOIN tb_devicebind AS db ON d.ID = db.BindId WHERE db.ID = 6";
                        DataTable dataTableGetAddressFour = mySQLHelper.ExecuteQuery(queryGetAddressFour);
                        string addressfour = "";
                        foreach (DataRow row in dataTableGetAddressFour.Rows)
                        {
                            addressfour = row["Address"].ToString();
                        }

                        Random random = new Random();
                        int randomNumber = random.Next(1, 4000);
                        siemensPPI.Write(addressfour, randomNumber);
                        int siemensValue = siemensPPI.ReadInt32(addressfour).Content;
                        siemensPPI.Close();
                        #endregion

                        #endregion

                        this.Dispatcher.Invoke(() =>
                        {
                            int onlineNumber = 0;
                            int infoNumber = 0;
                            int warnNumber = 0;
                            int errorNumber = 0;

                            #region 温湿度
                            if (valueTworesult != 0 && valueOneresult != 0)
                            {
                                if (valueTworesult > 30 || valueTworesult < 5 ||
                                    valueOneresult > 80 || valueOneresult < 30)
                                {
                                    img_One.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_SOneVa1.Text = valueTworesult.ToString();
                                    txt_SOneVa2.Text = valueOneresult.ToString();
                                    onlineNumber++;
                                    warnNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                               "FROM tb_historydata WHERE Name = '一号传感器';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_SOneU.Text = addressUsingPower.ToString();
                                    txt_SOneU.Text = (Convert.ToDecimal(txt_SOneU.Text) + (Convert.ToDecimal(txt_SOneP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_SOneP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_SOneVa1.Text)), 4);
                                    decimal data2 = Math.Round((Convert.ToDecimal(txt_SOneVa2.Text)), 4);
                                    mySQLHelper.InsertHistory("一号传感器", "点位1", usingpower, data1, dateTime, "Warn");
                                    mySQLHelper.InsertHistory("一号传感器", "点位2", usingpower, data2, dateTime, "Warn");
                                }
                                else
                                {
                                    img_One.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_SOneVa1.Text = valueTworesult.ToString();
                                    txt_SOneVa2.Text = valueOneresult.ToString();
                                    onlineNumber++;
                                    infoNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '一号传感器';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_SOneU.Text = addressUsingPower.ToString();
                                    txt_SOneU.Text = (Convert.ToDecimal(txt_SOneU.Text) + (Convert.ToDecimal(txt_SOneP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_SOneP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_SOneVa1.Text)), 4);
                                    decimal data2 = Math.Round((Convert.ToDecimal(txt_SOneVa2.Text)), 4);
                                    mySQLHelper.InsertHistory("一号传感器", "点位1", usingpower, data1, dateTime, "Info");
                                    mySQLHelper.InsertHistory("一号传感器", "点位2", usingpower, data2, dateTime, "Info");
                                }
                            }
                            else
                            {
                                img_One.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/disconnect.png"));
                                txt_SOneVa1.Text = "00.0";
                                txt_SOneVa2.Text = "00.0";
                                errorNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '一号传感器';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_SOneU.Text = addressUsingPower.ToString();
                            }
                            #endregion

                            #region 光照度
                            if (valueThreeresult != 0 && valueThreeresult != 0)
                            {
                                if (valueThreeresult > 1500 || valueThreeresult < 100)
                                {
                                    img_Two.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_STwoVa.Text = valueThreeresult.ToString();
                                    onlineNumber++;
                                    warnNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                               "FROM tb_historydata WHERE Name = '二号传感器';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_STwoU.Text = addressUsingPower.ToString();
                                    txt_STwoU.Text = (Convert.ToDecimal(txt_STwoU.Text) + (Convert.ToDecimal(txt_STwoP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_STwoP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_STwoVa.Text)), 4);
                                    mySQLHelper.InsertHistory("二号传感器", "点位1", usingpower, data1, dateTime, "Warn");
                                }
                                else
                                {
                                    img_Two.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_STwoVa.Text = valueThreeresult.ToString();
                                    onlineNumber++;
                                    infoNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '二号传感器';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_STwoU.Text = addressUsingPower.ToString();
                                    txt_STwoU.Text = (Convert.ToDecimal(txt_STwoU.Text) + (Convert.ToDecimal(txt_STwoP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_STwoP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_STwoVa.Text)), 4);
                                    mySQLHelper.InsertHistory("二号传感器", "点位1", usingpower, data1, dateTime, "Info");
                                }
                            }
                            else
                            {
                                img_One.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/disconnect.png"));
                                txt_STwoVa.Text = "0000";
                                errorNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '二号传感器';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_STwoU.Text = addressUsingPower.ToString();
                            }
                            #endregion

                            #region DO设备
                            if (valueDoresult == 0)
                            {
                                img_Three.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                txt_OCVa1.Text = "关"; txt_OCVa2.Text = "关";
                                onlineNumber++;
                                infoNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '三号开关量';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_OCU.Text = addressUsingPower.ToString();
                                txt_OCU.Text = (Convert.ToDecimal(txt_OCU.Text) + (Convert.ToDecimal(txt_OCP.Text) / 3600)).ToString("0.0000");
                                decimal usingpower = Math.Round((Convert.ToDecimal(txt_OCP.Text) / 3600), 4);
                                DateTime dateTime = DateTime.Now;
                                decimal data1 = 0;
                                decimal data2 = 0;
                                if (txt_OCVa1.Text == "关") { data1 = 0; } else { data1 = 1; };
                                if (txt_OCVa2.Text == "关") { data2 = 0; } else { data2 = 1; };
                                mySQLHelper.InsertHistory("三号开关量", "点位1", usingpower, data1, dateTime, "Info");
                                mySQLHelper.InsertHistory("三号开关量", "点位2", usingpower, data2, dateTime, "Info");
                            }
                            if (valueDoresult == 1)
                            {
                                img_Three.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                txt_OCVa1.Text = "开"; txt_OCVa2.Text = "关";
                                onlineNumber++;
                                infoNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '三号开关量';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_OCU.Text = addressUsingPower.ToString();
                                txt_OCU.Text = (Convert.ToDecimal(txt_OCU.Text) + (Convert.ToDecimal(txt_OCP.Text) / 3600)).ToString("0.0000");
                                decimal usingpower = Math.Round((Convert.ToDecimal(txt_OCP.Text) / 3600), 4);
                                DateTime dateTime = DateTime.Now;
                                decimal data1 = 0;
                                decimal data2 = 0;
                                if (txt_OCVa1.Text == "关") { data1 = 0; } else { data1 = 1; };
                                if (txt_OCVa2.Text == "关") { data2 = 0; } else { data2 = 1; };
                                mySQLHelper.InsertHistory("三号开关量", "点位1", usingpower, data1, dateTime, "Info");
                                mySQLHelper.InsertHistory("三号开关量", "点位2", usingpower, data2, dateTime, "Info");
                            }
                            if (valueDoresult == 2)
                            {
                                img_Three.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                txt_OCVa1.Text = "关"; txt_OCVa2.Text = "开";
                                onlineNumber++;
                                infoNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '三号开关量';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_OCU.Text = addressUsingPower.ToString();
                                txt_OCU.Text = (Convert.ToDecimal(txt_OCU.Text) + (Convert.ToDecimal(txt_OCP.Text) / 3600)).ToString("0.0000");
                                decimal usingpower = Math.Round((Convert.ToDecimal(txt_OCP.Text) / 3600), 4);
                                DateTime dateTime = DateTime.Now;
                                decimal data1 = 0;
                                decimal data2 = 0;
                                if (txt_OCVa1.Text == "关") { data1 = 0; } else { data1 = 1; };
                                if (txt_OCVa2.Text == "关") { data2 = 0; } else { data2 = 1; };
                                mySQLHelper.InsertHistory("三号开关量", "点位1", usingpower, data1, dateTime, "Info");
                                mySQLHelper.InsertHistory("三号开关量", "点位2", usingpower, data2, dateTime, "Info");
                            }
                            if (valueDoresult == 3)
                            {
                                img_Three.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                txt_OCVa1.Text = "开"; txt_OCVa2.Text = "开";
                                onlineNumber++;
                                infoNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '三号开关量';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_OCU.Text = addressUsingPower.ToString();
                                txt_OCU.Text = (Convert.ToDecimal(txt_OCU.Text) + (Convert.ToDecimal(txt_OCP.Text) / 3600)).ToString("0.0000");
                                decimal usingpower = Math.Round((Convert.ToDecimal(txt_OCP.Text) / 3600), 4);
                                DateTime dateTime = DateTime.Now;
                                decimal data1 = 0;
                                decimal data2 = 0;
                                if (txt_OCVa1.Text == "关") { data1 = 0; } else { data1 = 1; };
                                if (txt_OCVa2.Text == "关") { data2 = 0; } else { data2 = 1; };
                                mySQLHelper.InsertHistory("三号开关量", "点位1", usingpower, data1, dateTime, "Info");
                                mySQLHelper.InsertHistory("三号开关量", "点位2", usingpower, data2, dateTime, "Info");
                            }
                            #endregion

                            #region PLC设备
                            try
                            {
                                if (siemensValue > 4000 || siemensValue < 500)
                                {
                                    img_Four.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_PlcVa.Text = siemensValue.ToString();
                                    onlineNumber++;
                                    warnNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                              "FROM tb_historydata WHERE Name = '四号PLC设备';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_PlcU.Text = addressUsingPower.ToString();
                                    txt_PlcU.Text = (Convert.ToDecimal(txt_PlcU.Text) + (Convert.ToDecimal(txt_PlcP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_PlcP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_PlcVa.Text)), 4);
                                    mySQLHelper.InsertHistory("四号PLC设备", "点位1", usingpower, data1, dateTime, "Warn");
                                }
                                else
                                {
                                    img_Four.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/connect.png"));
                                    txt_PlcVa.Text = siemensValue.ToString();
                                    onlineNumber++;
                                    infoNumber++;
                                    string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                              "FROM tb_historydata WHERE Name = '四号PLC设备';";
                                    DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                    decimal addressUsingPower = 0;
                                    addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                    txt_PlcU.Text = addressUsingPower.ToString();
                                    txt_PlcU.Text = (Convert.ToDecimal(txt_PlcU.Text) + (Convert.ToDecimal(txt_PlcP.Text) / 3600)).ToString("0.0000");
                                    decimal usingpower = Math.Round((Convert.ToDecimal(txt_PlcP.Text) / 3600), 4);
                                    DateTime dateTime = DateTime.Now;
                                    decimal data1 = Math.Round((Convert.ToDecimal(txt_PlcVa.Text)), 4);
                                    mySQLHelper.InsertHistory("四号PLC设备", "点位1", usingpower, data1, dateTime, "Info");

                                }
                            }
                            catch
                            {
                                img_Four.Source = new BitmapImage(new Uri("pack://application:,,,/Pic/disconnect.png"));
                                txt_PlcVa.Text = "0000";
                                errorNumber++;
                                string queryGetUsingPower = "SELECT SUM(UsingPower) AS TotalUsingPower " +
                                                                "FROM tb_historydata WHERE Name = '四号PLC设备';";
                                DataTable dataTableGetUsingPower = mySQLHelper.ExecuteQuery(queryGetUsingPower);
                                decimal addressUsingPower = 0;
                                addressUsingPower = Math.Round(Convert.ToDecimal(dataTableGetUsingPower.Rows[0][0]), 4);
                                txt_PlcU.Text = addressUsingPower.ToString();
                            }
                            #endregion

                            #region Pie图刷新
                            if ((DateTime.Now - lastUpdateTime).TotalSeconds >= 5)
                            {
                                lastUpdateTime = DateTime.Now;
                                chart_Online.Series = new ISeries[] {
                                    new PieSeries<int> { Values = new int[] { onlineNumber },Name="Connect",
                                        Fill = new SolidColorPaint(new SKColor(170, 255, 86))},
                                    new PieSeries<int> { Values = new int[] { 4-onlineNumber },Name="Disconnect",
                                        Fill = new SolidColorPaint(new SKColor(255, 86, 86))},
                            };
                                chart_state.Series = new ISeries[]{
                                     new PieSeries<double> { Values = new double[] { infoNumber },Name="Info",
                                         Fill = new SolidColorPaint(new SKColor(86, 170, 255))},
                                     new PieSeries<double> { Values = new double[] { warnNumber },Name="Warn",
                                         Fill = new SolidColorPaint(new SKColor(255, 255, 86))},
                                    new PieSeries<double> { Values = new double[] { errorNumber},Name="Error",
                                        Fill = new SolidColorPaint(new SKColor(255, 86, 86))},
                           };
                            }
                            #endregion
                        });
                        #endregion

                        Thread.Sleep(1000);
                    }

                }
                catch { }
                

            }).ConfigureAwait(false);
        }

        public async void ControlTask()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(3000);
                        #region 控制策略判断

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            string device = row["Device"].ToString();
                            string point = row["Point"].ToString();
                            //条件
                            string condition = row["Condition"].ToString();
                            //阈值
                            string threshold = row["Threshold"].ToString();
                            string ocdo = row["Do"].ToString();
                            string openclose = row["OpenClose"].ToString();
                            int status = Convert.ToInt32(row["Status"]);

                            if (status == 1)
                            {
                                string latestQuery = $"SELECT Data FROM tb_historydata WHERE Name = '{device}' AND Point = '{point}' ORDER BY ID DESC LIMIT 1";
                                DataTable latestData = mySQLHelper.ExecuteQuery(latestQuery);
                                if (latestData.Rows.Count > 0)
                                {
                                    string latestRow = latestData.Rows[0][0].ToString();

                                    NCalc.Expression expression = new NCalc.Expression($"{latestRow} {condition} {threshold}");
                                    bool result = Convert.ToBoolean(expression.Evaluate());
                                    if (result)
                                    {
                                        if (openclose == "开")
                                        {
                                            if (ocdo == "DO1")
                                            {
                                                //DO1开指令
                                                byte[] doOneOpenBytes = { 0x03, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8D, 0xD8 };
                                                stream.WriteAsync(doOneOpenBytes, 0, doOneOpenBytes.Length);
                                                stream.Flush();
                                                Thread.Sleep(200);
                                            }
                                            else
                                            {
                                                //DO2开指令
                                                byte[] doTwoOpenBytes = { 0x03, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDC, 0x18 };
                                                stream.WriteAsync(doTwoOpenBytes, 0, doTwoOpenBytes.Length);
                                                stream.Flush();
                                                Thread.Sleep(200);
                                            }
                                        }
                                        else
                                        {
                                            if (ocdo == "DO1")
                                            {
                                                //DO1关指令
                                                byte[] doOneCloseBytes = { 0x03, 0x05, 0x00, 0x00, 0x00, 0x00, 0xCC, 0x28 };
                                                stream.WriteAsync(doOneCloseBytes, 0, doOneCloseBytes.Length);
                                                stream.Flush();
                                                Thread.Sleep(200);
                                            }
                                            else
                                            {
                                                //DO2关指令
                                                byte[] doTwoCloseBytes = { 0x03, 0x05, 0x00, 0x01, 0x00, 0x00, 0x9D, 0xE8 };
                                                stream.WriteAsync(doTwoCloseBytes, 0, doTwoCloseBytes.Length);
                                                stream.Flush();
                                                Thread.Sleep(200);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }

                            #endregion

                        }

                    }
                }
                catch { }
             
            });
        }

        public static ushort CalculateCRC(byte[] data)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) == 1)
                    {
                        crc >>= 1;
                        crc ^= 0xA001; // CRC-16 标准多项式
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }
    };
}

