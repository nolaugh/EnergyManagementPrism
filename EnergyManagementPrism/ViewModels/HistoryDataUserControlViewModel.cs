using EnergyManagementPrism.Model;
using EnergyManagementPrism.PublicClass;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnergyManagementPrism.ViewModels
{
    public class HistoryDataUserControlViewModel : BindableBase
    {
        MySQLHelper mySQLHelper = new MySQLHelper();

        private ObservableCollection<HistoryDataModel> _historydata = new ObservableCollection<HistoryDataModel>();
        public ObservableCollection<HistoryDataModel> HistoryDataItems
        {
            get { return _historydata; }
            set { SetProperty(ref _historydata, value); }
        }

        private string _devicename;
        public string DeviceName
        {
            get { return _devicename; }
            set { SetProperty(ref _devicename, value); }
        }

        private string _devicepoint;
        public string DevicePoint
        {
            get { return _devicepoint; }
            set { SetProperty(ref _devicepoint, value); }
        }

        private DateTime _starttime;
        public DateTime StartTime
        {
            get { return _starttime; }
            set { SetProperty(ref _starttime, value); }
        }

        private DateTime _endtime;
        public DateTime EndTime
        {
            get { return _endtime; }
            set { SetProperty(ref _endtime, value); }
        }

        public HistoryDataUserControlViewModel()
        {

        }

        public DelegateCommand LoadedCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string query = "SELECT * FROM tb_historydata";
                    DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                    HistoryDataItems.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);

                        AddData(
                                  id,
                                  row["Name"].ToString(),
                                  row["Point"].ToString(),
                                  Convert.ToDecimal(row["UsingPower"]),
                                  Convert.ToDecimal(row["Data"]),
                                  Convert.ToDateTime(row["Time"]),
                                  row["LogLevel"].ToString());

                    }
                });
            }).ConfigureAwait(false);
        });

        //查询数据指令
        public DelegateCommand SelectCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (DeviceName == null || DeviceName == ""
                    || DevicePoint == null || DevicePoint == "")
                    {
                        HandyControl.Controls.MessageBox.Error("请选择条件！", "提示");
                    }
                    else
                    {
                        string getDataQuery = "SELECT * FROM tb_historydata " +
                     "WHERE Name = '" + DeviceName + "' AND Point = '" + DevicePoint + "' " +
                     "AND Time >= '" + StartTime + "' AND Time <= '" + EndTime + "'";

                        DataTable dataTableGetData = mySQLHelper.ExecuteQuery(getDataQuery);
                        HistoryDataItems.Clear();
                        foreach (DataRow row in dataTableGetData.Rows)
                        {

                            int id = Convert.ToInt32(row["ID"]);

                            AddData(
                                      id,
                                      row["Name"].ToString(),
                                      row["Point"].ToString(),
                                      Convert.ToDecimal(row["UsingPower"]),
                                      Convert.ToDecimal(row["Data"]),
                                      Convert.ToDateTime(row["Time"]),
                                      row["LogLevel"].ToString());
                        }


                    }
                });
            }).ConfigureAwait(false);
        });

        // 添加历史数据方法
        public void AddData(int id, string name, string point, decimal usingpower, decimal data, DateTime datetime,string logelevel)
        {
            HistoryDataItems.Add(new HistoryDataModel
            {
                ID = id,
                Name = name,
                Point = point,
                UsingPower = usingpower,
                Data = data,
                Time = datetime,
                LogLevel = logelevel
            });
        }

    }
}
