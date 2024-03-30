using EnergyManagementPrism.Model;
using EnergyManagementPrism.PublicClass;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnergyManagementPrism.ViewModels
{
    public class ControlPolicyUserControlViewModel:BindableBase
    {
        MySQLHelper mySQLHelper =new MySQLHelper();

        private ObservableCollection<ControlModel> _controlitems = new ObservableCollection<ControlModel>();
        public ObservableCollection<ControlModel> ControlItems
        {
            get { return _controlitems; }
            set { SetProperty(ref _controlitems, value); }
        }

        private string _tdevice;
        public string TDevice
        {
            get { return _tdevice; }
            set { SetProperty(ref _tdevice, value); }
        }

        private string _tpoint;
        public string TPoint
        {
            get { return _tpoint; }
            set { SetProperty(ref _tpoint, value); }
        }

        private string _tcondition;
        public string TCondition
        {
            get { return _tcondition; }
            set { SetProperty(ref _tcondition, value); }
        }

        private string _tthreshold;
        public string TThreshold
        {
            get { return _tthreshold; }
            set { SetProperty(ref _tthreshold, value); }
        }

        private string _tdo;
        public string TDo
        {
            get { return _tdo; }
            set { SetProperty(ref _tdo, value); }
        }

        private string _topenclose;
        public string TOpenClose
        {
            get { return _topenclose; }
            set { SetProperty(ref _topenclose, value); }
        }

        private ControlModel _controlselectitem;
        public ControlModel ControlSelectItem
        {

            get { return _controlselectitem; }
            set { SetProperty(ref _controlselectitem, value); }
        }


        public ControlPolicyUserControlViewModel() 
        {
            
        }

        //加载指令
        public DelegateCommand LoadedCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string query = "SELECT * FROM tb_control";
                    DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                    ControlItems.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        if (!ControlItems.Any(d => d.ID == id))
                        {
                            AddData(
                                id,
                                row["Device"].ToString(),
                                row["Point"].ToString(),
                                row["Condition"].ToString(),
                                row["Threshold"].ToString(),
                                row["Do"].ToString(),
                                row["OpenClose"].ToString(),
                                Convert.ToInt32(row["Status"])
                               );
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

        // 添加控制指令方法
        public void AddData(int id, string device,string point,string condition,string threshold,string ocdo,string openclose,int status)
        {
            ControlItems.Add(new ControlModel
            {
                ID = id,
                Device = device,
                Point = point,
                Condition = condition,
                Threshold = threshold,
                Do=ocdo,
                OpenClose=openclose,
                Status=status
            });
        }

        // 新增策略指令
        public DelegateCommand NewControlCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.InsertControl(TDevice, TPoint, TCondition, TThreshold, TDo, TOpenClose, 0);
                        HandyControl.Controls.MessageBox.Info("新增策略成功！", "提示");

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        ControlItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!ControlItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Device"].ToString(),
                                    row["Point"].ToString(),
                                    row["Condition"].ToString(),
                                    row["Threshold"].ToString(),
                                    row["Do"].ToString(),
                                    row["OpenClose"].ToString(),
                                    Convert.ToInt32(row["Status"])
                                   );
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("新增策略失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //修改策略指令
        public DelegateCommand UpdateControlCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.UpdateControl(ControlSelectItem.ID,TDevice, TPoint, TCondition, TThreshold, TDo, TOpenClose, 0);
                        HandyControl.Controls.MessageBox.Info("更新策略成功！", "提示");

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        ControlItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!ControlItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Device"].ToString(),
                                    row["Point"].ToString(),
                                    row["Condition"].ToString(),
                                    row["Threshold"].ToString(),
                                    row["Do"].ToString(),
                                    row["OpenClose"].ToString(),
                                    Convert.ToInt32(row["Status"])
                                   );
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("更新策略失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //删除策略指令
        public DelegateCommand DeleteControlCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.DeleteControl(ControlSelectItem.ID);
                        HandyControl.Controls.MessageBox.Info("删除策略成功！", "提示");

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        ControlItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!ControlItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Device"].ToString(),
                                    row["Point"].ToString(),
                                    row["Condition"].ToString(),
                                    row["Threshold"].ToString(),
                                    row["Do"].ToString(),
                                    row["OpenClose"].ToString(),
                                    Convert.ToInt32(row["Status"])
                                   );
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("删除策略失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //更新策略启动状态指令
        public DelegateCommand StartContorlCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.UpdateControl(ControlSelectItem.ID,1);
                        HandyControl.Controls.MessageBox.Info("启动策略成功！", "提示");

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        ControlItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!ControlItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Device"].ToString(),
                                    row["Point"].ToString(),
                                    row["Condition"].ToString(),
                                    row["Threshold"].ToString(),
                                    row["Do"].ToString(),
                                    row["OpenClose"].ToString(),
                                    Convert.ToInt32(row["Status"])
                                   );
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("启动策略失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //更新策略关闭状态指令
        public DelegateCommand StopContorlCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.UpdateControl(ControlSelectItem.ID, 0);
                        HandyControl.Controls.MessageBox.Info("停止策略成功！", "提示");

                        string query = "SELECT * FROM tb_control";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        ControlItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!ControlItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Device"].ToString(),
                                    row["Point"].ToString(),
                                    row["Condition"].ToString(),
                                    row["Threshold"].ToString(),
                                    row["Do"].ToString(),
                                    row["OpenClose"].ToString(),
                                    Convert.ToInt32(row["Status"])
                                   );
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("停止策略失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });
    }
}
