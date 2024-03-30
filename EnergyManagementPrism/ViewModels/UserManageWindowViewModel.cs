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
    public  class UserManageWindowViewModel:BindableBase
    {
        MySQLHelper mySQLHelper = new MySQLHelper();

        public UserManageWindowViewModel() 
        {
            
        }

        private ObservableCollection<UserModel> _useritems = new ObservableCollection<UserModel>();
        public ObservableCollection<UserModel> UserItems
        {
            get { return _useritems; }
            set { SetProperty(ref _useritems, value); }
        }

        private string _taccount = "0";

        public string TAccount
        {
            get { return _taccount; }
            set { SetProperty(ref _taccount, value); }
        }

        private string _tpassword = "0";

        public string TPassword
        {
            get { return _tpassword; }
            set { SetProperty(ref _tpassword, value); }
        }

        private int _tpermissions =0;

        public int TPermissions
        {
            get { return _tpermissions; }
            set { SetProperty(ref _tpermissions, value); }
        }

        private UserModel _userselectItems;
        public UserModel UserSelectItems
        {

            get { return _userselectItems; }
            set { SetProperty(ref _userselectItems, value); }
        }

        //加载指令
        public DelegateCommand LoadedCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string query = "SELECT * FROM tb_login";
                    DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                    UserItems.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        if (!UserItems.Any(d => d.ID == id))
                        {
                            AddData(
                                id,
                                row["Account"].ToString(),
                                row["Password"].ToString(),
                                Convert.ToInt32(row["Permissions"]));
                        }
                    }
                });
            }).ConfigureAwait(false);
        });

        // 添加用户方法
        public void AddData(int id, string account, string password, int permissions)
        {
            UserItems.Add(new UserModel
            {
                ID = id,
                Account = account,
                Password= password,
                Permissions = permissions
            });
        }

        public DelegateCommand NewUserCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.InsertUser(TAccount, TPassword, TPermissions);
                        HandyControl.Controls.MessageBox.Info("新增用户成功！", "提示");

                        string query = "SELECT * FROM tb_login";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        UserItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!UserItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Account"].ToString(),
                                    row["Password"].ToString(),
                                    Convert.ToInt32(row["Permissions"]));
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("新增用户失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        public DelegateCommand UpdateUserCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.UpdateUser(UserSelectItems.ID, TAccount, TPassword, TPermissions);
                        HandyControl.Controls.MessageBox.Info("更新用户成功！", "提示");

                        string query = "SELECT * FROM tb_login";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        UserItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!UserItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Account"].ToString(),
                                    row["Password"].ToString(),
                                    Convert.ToInt32(row["Permissions"]));
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("更新用户失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });

        public DelegateCommand DeleteUserCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        mySQLHelper.DeleteUser(UserSelectItems.ID);
                        HandyControl.Controls.MessageBox.Info("删除用户成功！", "提示");

                        string query = "SELECT * FROM tb_login";
                        DataTable dataTable = mySQLHelper.ExecuteQuery(query);
                        UserItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int id = Convert.ToInt32(row["ID"]);
                            if (!UserItems.Any(d => d.ID == id))
                            {
                                AddData(
                                    id,
                                    row["Account"].ToString(),
                                    row["Password"].ToString(),
                                    Convert.ToInt32(row["Permissions"]));
                            }
                        }
                    }
                    catch
                    {
                        HandyControl.Controls.MessageBox.Error("删除用户失败！", "提示");
                    }
                });
            }).ConfigureAwait(false);
        });
    }
}
