using EnergyManagementPrism.Model;
using EnergyManagementPrism.PublicClass;
using EnergyManagementPrism.Views;
using Org.BouncyCastle.Asn1.X509;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EnergyManagementPrism.ViewModels
{
    public class DeviceManageUserControlViewModel:BindableBase
    {
        MySQLHelper mySQLHelper=new MySQLHelper();

        private ObservableCollection<Device> _deviceItems = new ObservableCollection<Device>();
        public ObservableCollection<Device> DeviceItems
        {
            get { return _deviceItems; }
            set { SetProperty(ref _deviceItems, value); }
        }

        private int _tid=0;

        public int TID
        {
            get { return _tid; }
            set { SetProperty(ref _tid, value); }
        }

        private string _tname="0";

        public string TName
        {
            get { return _tname; }
            set { SetProperty(ref _tname, value); }
        }

        private decimal _tvolotage=0;

        public decimal TVolotage
        {
            get { return _tvolotage; }
            set { SetProperty(ref _tvolotage, value); }
        }

        private decimal _telectricit=0;

        public decimal TElectricit
        {
            get { return _telectricit; }
            set { SetProperty(ref _telectricit, value); }
        }

        private decimal _tpower=0;

        public decimal TPower
        {
            get { return _tpower; }
            set { SetProperty(ref _tpower, value); }
        }

        private string _taddress="0";

        public string TAddress
        {
            get { return _taddress; }
            set { SetProperty(ref _taddress, value); }
        }


        private Device _deviceSelectItem;
        public Device DeviceSelectItem
        {

            get { return _deviceSelectItem; }
            set { SetProperty(ref _deviceSelectItem, value); }
        }


        public DeviceManageUserControlViewModel()
        {

        }

        //加载指令
        public DelegateCommand LoadedCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string query = "SELECT * FROM tb_device";
                    DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                    DeviceItems.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        if (!DeviceItems.Any(d => d.ID == id))
                        {
                            AddData(
                                id,
                                row["Name"].ToString(),
                                Convert.ToDecimal(row["Volotage"]),
                                Convert.ToDecimal(row["Electricit"]),
                                Convert.ToDecimal(row["Power"]),
                                row["Address"].ToString());
                        }
                    }

                });
            }).ConfigureAwait(false);
        });

        // 添加设备方法
        public void AddData(int id, string name, decimal volotage, decimal electricit, decimal power, string address)
        {
            DeviceItems.Add(new Device
            {
                ID = id,
                Name = name,
                Volotage = volotage,
                Electricit = electricit,
                Power = power,
                Address = address
            });
        }

        //新增设备指令
        public DelegateCommand NewDeviceCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.InsertDevice(TName, TVolotage, TElectricit, TPower, TAddress);
                        HandyControl.Controls.MessageBox.Info("新增设备成功！", "提示");

                        string query = "SELECT * FROM tb_device";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        DeviceItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!DeviceItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Name"].ToString(),
                                    Convert.ToDecimal(row["Volotage"]),
                                    Convert.ToDecimal(row["Electricit"]),
                                    Convert.ToDecimal(row["Power"]),
                                    row["Address"].ToString());
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("新增设备失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //更新设备指令
        public DelegateCommand UpdateDeviceCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.UpdateDevice(TID,TName, TVolotage, TElectricit, TPower, TAddress);
                        HandyControl.Controls.MessageBox.Info("更新设备成功！", "提示");

                        string query = "SELECT * FROM tb_device";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        DeviceItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!DeviceItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Name"].ToString(),
                                    Convert.ToDecimal(row["Volotage"]),
                                    Convert.ToDecimal(row["Electricit"]),
                                    Convert.ToDecimal(row["Power"]),
                                    row["Address"].ToString());
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("更新设备失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //删除设备指令
        public DelegateCommand DeleteDeviceCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.DeleteDevice(TID);
                        HandyControl.Controls.MessageBox.Info("删除设备成功！", "提示");

                        string query = "SELECT * FROM tb_device";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        DeviceItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!DeviceItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Name"].ToString(),
                                    Convert.ToDecimal(row["Volotage"]),
                                    Convert.ToDecimal(row["Electricit"]),
                                    Convert.ToDecimal(row["Power"]),
                                    row["Address"].ToString());
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("删除设备失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        //配置管理指令
        public DelegateCommand ManageDeviceCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConfigurationWindow configurationWindow = new ConfigurationWindow();
                    configurationWindow.ShowDialog();
                });
            }).ConfigureAwait(false);
        });
    }
}
