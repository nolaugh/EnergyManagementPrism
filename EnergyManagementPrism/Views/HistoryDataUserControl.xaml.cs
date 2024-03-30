using EnergyManagementPrism.PublicClass;
using EnergyManagementPrism.ViewModels;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Prism.Services.Dialogs;
using MySql.Data.MySqlClient;


namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// HistoryDataUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDataUserControl : UserControl
    {
        MySQLHelper mySQLHelper=new MySQLHelper();
        public HistoryDataUserControl()
        {
            InitializeComponent();
            HistoryDataUserControlViewModel historyDataUserControlViewModel = new HistoryDataUserControlViewModel();
            this.DataContext = historyDataUserControlViewModel;
        }

        private void cmb_Sensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_Sensor.SelectedIndex == 0 || cmb_Sensor.SelectedIndex == 2)
            {
                cmb_Point.ItemsSource = new[] { "点位1", "点位2" };
            }
            else
            {
                cmb_Point.ItemsSource = new[] { "点位1" };
            }
        }

        private async void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        string DeviceName = cmb_Sensor.Text.ToString();
                        string DevicePoint = cmb_Point.Text.ToString();
                        DateTime StartTime = DTP_StartTime.SelectedDateTime.Value;
                        DateTime EndTime = DTP_EndTime.SelectedDateTime.Value;
                        string getDataQuery = "SELECT * FROM tb_historydata " +
                         "WHERE Name = '" + DeviceName + "' AND Point = '" + DevicePoint + "' " +
                         "AND Time >= '" + StartTime + "' AND Time <= '" + EndTime + "'";
                        System.Data.DataTable dataTableGetData = mySQLHelper.ExecuteQuery(getDataQuery);

                        dg_History.ItemsSource = dataTableGetData.DefaultView;

                        // 从查询结果中提取时间和数据列
                        var times = dataTableGetData.AsEnumerable().Select(r => Convert.ToDateTime(r["Time"]));
                        var dataValues = dataTableGetData.AsEnumerable().Select(r => Convert.ToDouble(r["Data"]));
                        double maxData = dataTableGetData.AsEnumerable().Select(r => Convert.ToDouble(r["Data"])).Max();

                        // 根据时间和数据列生成曲线
                        chart_CHistory.Series = new LiveChartsCore.ISeries[]
                        {
                        new LineSeries<double> { Values = dataValues.ToArray() }
                        };

                        // 设置 X 轴的标签为时间
                        chart_CHistory.XAxes = new[]
                        {
                        new LiveChartsCore.SkiaSharpView.Axis
                        {
                            Labels = times.Select(t => t.ToString("yyyy-MM-dd HH:mm:ss")).ToArray(),
                            TextSize = 20,
                            LabelsPaint = new SolidColorPaint(new SkiaSharp.SKColor(86, 170, 255))
                        }
                    };

                        // 设置 Y 轴的自定义分隔符
                        chart_CHistory.YAxes = new[]
                        {
                        new LiveChartsCore.SkiaSharpView.Axis
                        {
                            CustomSeparators = GetCustomSeparators(maxData),
                            TextSize = 20,
                            LabelsPaint = new SolidColorPaint(new SkiaSharp.SKColor(86, 170, 255))
                        }
                };
                        HandyControl.Controls.MessageBox.Info("查询成功！", "提示");
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Info("查询失败，请检查！", "提示");
                    }
                });
            });
        }

        // 定义一个方法来生成自定义分隔符数组
        private double[] GetCustomSeparators(double maxValue)
        {
            List<double> separators = new List<double>();
            double step = maxValue / 5; // 将最大值分为 5 个区间
            for (int i = 0; i <= 5; i++)
            {
                separators.Add(i * step);
            }
            return separators.ToArray();
        }

        private void btn_Ex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建Excel应用程序对象
                Excel.Application excelApp = new Excel.Application();
                excelApp.Visible = true;

                // 创建工作簿对象
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                // 将数据写入Excel工作表
                for (int i = 0; i < dg_History.Items.Count; i++)
                {
                    var item = dg_History.Items[i] as DataRowView;
                    if (item != null)
                    {
                        for (int j = 0; j < dg_History.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 1, j + 1] = item.Row[j].ToString();
                        }
                    }
                }

                // 保存工作簿
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                workbook.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                HandyControl.Controls.MessageBox.Info("导出成功！", "提示");
            }
            catch
            {
                HandyControl.Controls.MessageBox.Error("导出失败，请检查！", "提示");
            } 
        }

        private void btn_Im_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel 文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            openFileDialog.Title = "选择要导入的 Excel 文件";

            // 如果用户点击了确定按钮
            if (openFileDialog.ShowDialog() == true)
            {
                // 打开 Excel 文件
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(openFileDialog.FileName);
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                // 读取数据
                System.Data.DataTable dataTable = new System.Data.DataTable();
                int rowCount = worksheet.UsedRange.Rows.Count;
                int colCount = worksheet.UsedRange.Columns.Count;
                for (int i = 1; i <= colCount; i++)
                {
                    dataTable.Columns.Add($"Column{i}");
                }

                for (int i = 1; i <= rowCount; i++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int j = 1; j <= colCount; j++)
                    {
                        row[j - 1] = worksheet.Cells[i, j].Value.ToString();
                    }
                    dataTable.Rows.Add(row);
                }

                // 关闭 Excel 进程
                workbook.Close();
                excelApp.Quit();

                // 执行插入逻辑
                ImportData(dataTable);
            }
        }

        private void ImportData(System.Data.DataTable dataTable)
        {
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (int.TryParse(row[0].ToString(), out int id))
                    {
                        if (!CheckIdConflict(id))
                        {
                            string name = row[1].ToString();
                            string point = row[2].ToString();
                            decimal value1 = decimal.TryParse(row[3].ToString(), out decimal val1) ? val1 : 0;
                            decimal value2 = decimal.TryParse(row[4].ToString(), out decimal val2) ? val2 : 0;
                            string dateStr = row[5].ToString();
                            DateTime time = DateTime.TryParse(dateStr, out DateTime dt) ? dt : DateTime.Now;
                            string value3 = row[6].ToString();

                            // 插入数据到数据库
                            string insertQuery = $"INSERT INTO tb_historydata (Name, Point, UsingPower, Data,Time,LogLevel) VALUES ('{name}','{point}', {value1}, {value2}, '{time.ToString("yyyy-MM-dd HH:mm:ss")}','{value3}')";
                            mySQLHelper.ExecuteQuery(insertQuery);
                        }
                    }
                }
                HandyControl.Controls.MessageBox.Info("导入成功！", "提示");
            }
            catch
            {
                HandyControl.Controls.MessageBox.Error("导入失败，请检查！", "提示");
            }
        }

        private bool CheckIdConflict(int id)
        {
            string checkQuery = $"SELECT COUNT(*) FROM tb_historydata WHERE ID = {id}";
            System.Data.DataTable dataTable = mySQLHelper.ExecuteQuery(checkQuery);
            if (dataTable.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
