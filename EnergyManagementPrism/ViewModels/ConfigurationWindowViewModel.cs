using EnergyManagementPrism.Model;
using EnergyManagementPrism.PublicClass;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnergyManagementPrism.ViewModels
{
    public class ConfigurationWindowViewModel : BindableBase
    {
        MySQLHelper mySQLHelper = new MySQLHelper();

        private ObservableCollection<Device> _cdeviceItems = new ObservableCollection<Device>();
        public ObservableCollection<Device> CDeviceItems
        {
            get { return _cdeviceItems; }
            set { SetProperty(ref _cdeviceItems, value); }
        }

        private ObservableCollection<int> _itemcmb = new ObservableCollection<int>();
        public ObservableCollection<int> ItemCmb
        {
            get { return _itemcmb; }
            set { SetProperty(ref _itemcmb, value); }
        }

        private string _onepointone;
        public string OnePiontOne
        {
            get { return _onepointone; }
            set { SetProperty(ref _onepointone, value); }
        }

        private string _onepointtwo;
        public string OnePiontTwo
        {
            get { return _onepointtwo; }
            set { SetProperty(ref _onepointtwo, value); }
        }

        private string _twopointone;
        public string TwoPiontOne
        {
            get { return _twopointone; }
            set { SetProperty(ref _twopointone, value); }
        }

        private string _twopointtwo;
        public string TwoPiontTwo
        {
            get { return _twopointtwo; }
            set { SetProperty(ref _twopointtwo, value); }
        }

        private string _threepointone;
        public string ThreePiontOne
        {
            get { return _threepointone; }
            set { SetProperty(ref _threepointone, value); }
        }

        private string _threepointtwo;
        public string ThreePiontTwo
        {
            get { return _threepointtwo; }
            set { SetProperty(ref _threepointtwo, value); }
        }

        private string _fourpointone;
        public string FourPiontOne
        {
            get { return _fourpointone; }
            set { SetProperty(ref _fourpointone, value); }
        }

        private string _fourpointtwo;
        public string FourPiontTwo
        {
            get { return _fourpointtwo; }
            set { SetProperty(ref _fourpointtwo, value); }
        }

        public ConfigurationWindowViewModel() 
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
                    CDeviceItems.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        if (!CDeviceItems.Any(d => d.ID == id))
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

                    string queryComBoBox = "SELECT ID FROM tb_device";
                    DataTable dataTableComBoBox = mySQLHelper.ExecuteQuery(queryComBoBox);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        ItemCmb.Add(id); 
                    }

                    string queryBind = "SELECT BindId FROM tb_deviceBind";
                    DataTable dataTableBind = mySQLHelper.ExecuteQuery(queryBind);
                    if (dataTableBind.Rows.Count >= 0)
                    {
                        OnePiontOne = dataTableBind.Rows[0]["BindId"].ToString();
                        OnePiontTwo = dataTableBind.Rows[1]["BindId"].ToString();
                        TwoPiontOne = dataTableBind.Rows[2]["BindId"].ToString();
                        ThreePiontOne = dataTableBind.Rows[3]["BindId"].ToString();
                        ThreePiontTwo = dataTableBind.Rows[4]["BindId"].ToString();
                        FourPiontOne = dataTableBind.Rows[5]["BindId"].ToString();
                    }


                });
            }).ConfigureAwait(false);
        });

        // 添加设备方法
        public void AddData(int id, string name, decimal volotage, decimal electricit, decimal power, string address)
        {
            CDeviceItems.Add(new Device
            {
                ID = id,
                Name = name,
                Volotage = volotage,
                Electricit = electricit,
                Power = power,
                Address = address
            });
        }

        //一号传感器绑定指令
        public DelegateCommand BindOne => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (OnePiontOne == "" || OnePiontOne == null
                    || OnePiontTwo == "" || OnePiontTwo == null)
                    {
                        HandyControl.Controls.MessageBox.Error("请选择绑定ID！", "提示");
                    }
                    else
                    {
                        try
                        {
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(OnePiontOne), 1);
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(OnePiontTwo), 2);
                            HandyControl.Controls.MessageBox.Info("绑定成功！", "提示");
                            string queryBind = "SELECT BindId FROM tb_deviceBind";
                            DataTable dataTableBind = mySQLHelper.ExecuteQuery(queryBind);
                            if (dataTableBind.Rows.Count >= 0)
                            {
                                OnePiontOne = dataTableBind.Rows[0]["BindId"].ToString();
                                OnePiontTwo = dataTableBind.Rows[1]["BindId"].ToString();
                                TwoPiontOne = dataTableBind.Rows[2]["BindId"].ToString();
                                ThreePiontOne = dataTableBind.Rows[3]["BindId"].ToString();
                                ThreePiontTwo = dataTableBind.Rows[4]["BindId"].ToString();
                                FourPiontOne = dataTableBind.Rows[5]["BindId"].ToString();
                            }
                        }
                        catch
                        {
                            HandyControl.Controls.MessageBox.Error("绑定失败！", "提示");
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

        //二号传感器绑定指令
        public DelegateCommand BindTwo => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (TwoPiontOne == "" || TwoPiontOne == null)
                    {
                        HandyControl.Controls.MessageBox.Error("请选择绑定ID！", "提示");
                    }
                    else
                    {
                        try
                        {
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(TwoPiontOne), 3);
                            HandyControl.Controls.MessageBox.Info("绑定成功！", "提示");
                            string queryBind = "SELECT BindId FROM tb_deviceBind";
                            DataTable dataTableBind = mySQLHelper.ExecuteQuery(queryBind);
                            if (dataTableBind.Rows.Count >= 0)
                            {
                                OnePiontOne = dataTableBind.Rows[0]["BindId"].ToString();
                                OnePiontTwo = dataTableBind.Rows[1]["BindId"].ToString();
                                TwoPiontOne = dataTableBind.Rows[2]["BindId"].ToString();
                                ThreePiontOne = dataTableBind.Rows[3]["BindId"].ToString();
                                ThreePiontTwo = dataTableBind.Rows[4]["BindId"].ToString();
                                FourPiontOne = dataTableBind.Rows[5]["BindId"].ToString();
                            }
                        }
                        catch
                        {
                            HandyControl.Controls.MessageBox.Error("绑定失败！", "提示");
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

        //三号开关量绑定指令
        public DelegateCommand BindThree => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ThreePiontOne == "" || ThreePiontOne == null ||
                    ThreePiontTwo == "" || ThreePiontTwo == null)
                    {
                        HandyControl.Controls.MessageBox.Error("请选择绑定ID！", "提示");
                    }
                    else
                    {
                        try
                        {
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(ThreePiontOne), 4);
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(ThreePiontTwo), 5);
                            HandyControl.Controls.MessageBox.Info("绑定成功！", "提示");
                            string queryBind = "SELECT BindId FROM tb_deviceBind";
                            DataTable dataTableBind = mySQLHelper.ExecuteQuery(queryBind);
                            if (dataTableBind.Rows.Count >= 0)
                            {
                                OnePiontOne = dataTableBind.Rows[0]["BindId"].ToString();
                                OnePiontTwo = dataTableBind.Rows[1]["BindId"].ToString();
                                TwoPiontOne = dataTableBind.Rows[2]["BindId"].ToString();
                                ThreePiontOne = dataTableBind.Rows[3]["BindId"].ToString();
                                ThreePiontTwo = dataTableBind.Rows[4]["BindId"].ToString();
                                FourPiontOne = dataTableBind.Rows[5]["BindId"].ToString();
                            }
                        }
                        catch
                        {
                            HandyControl.Controls.MessageBox.Error("绑定失败！", "提示");
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

        //四号PLC设备绑定指令
        public DelegateCommand BindFour => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (FourPiontOne == "" || FourPiontOne == null )
                    {
                        HandyControl.Controls.MessageBox.Error("请选择绑定ID！", "提示");
                    }
                    else
                    {
                        try
                        {
                            mySQLHelper.UpdateDeviceBind(Convert.ToInt32(FourPiontOne), 6);
                            HandyControl.Controls.MessageBox.Info("绑定成功！", "提示");
                            string queryBind = "SELECT BindId FROM tb_deviceBind";
                            DataTable dataTableBind = mySQLHelper.ExecuteQuery(queryBind);
                            if (dataTableBind.Rows.Count >= 0)
                            {
                                OnePiontOne = dataTableBind.Rows[0]["BindId"].ToString();
                                OnePiontTwo = dataTableBind.Rows[1]["BindId"].ToString();
                                TwoPiontOne = dataTableBind.Rows[2]["BindId"].ToString();
                                ThreePiontOne = dataTableBind.Rows[3]["BindId"].ToString();
                                ThreePiontTwo = dataTableBind.Rows[4]["BindId"].ToString();
                                FourPiontOne = dataTableBind.Rows[5]["BindId"].ToString();
                            }
                        }
                        catch
                        {
                            HandyControl.Controls.MessageBox.Error("绑定失败！", "提示");
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

    }
}
