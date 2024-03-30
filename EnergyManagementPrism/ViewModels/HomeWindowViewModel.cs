using EnergyManagementPrism.Model;
using EnergyManagementPrism.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnergyManagementPrism.ViewModels
{
    public class HomeWindowViewModel : BindableBase
    {
        public MonitorUserControl monitorUserControl = new MonitorUserControl();

        public DeviceManageUserControl deviceManageUserControl = new DeviceManageUserControl();

        public ControlPolicyUserControl controlPolicyUserControl = new ControlPolicyUserControl();

        public HistoryDataUserControl  historyDataUserControl = new HistoryDataUserControl();

        public UserControl _userContent;

        public UserControl UserContent
        {
            get { return _userContent; }
            set { SetProperty(ref _userContent, value); }
        }

        private int _permissions;
        public int Permissions
        {
            get { return _permissions; }
            set { SetProperty(ref _permissions, value); }
        }

        private string _userKind;
        public string UserKind
        {
            get { return _userKind; }
            set { SetProperty(ref _userKind, value); }
        }
   

        //HomeWindowViewModel入口
        public HomeWindowViewModel(int permissions)
        {
            Permissions = permissions;

            if (Permissions == 1)
            {
                UserKind = "当前操作者：管理员";
            }
            else
            {
                UserKind = "当前操作者：普通用户";
            }

            //默认界面
            UserContent = monitorUserControl;
        }
 
        //跳转实时监测页面
        public DelegateCommand Mointorcmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserContent = monitorUserControl;
                });
            }).ConfigureAwait(false);
        });

        //跳转控制策略页面
        public DelegateCommand Controlcmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserContent = controlPolicyUserControl;
                });
            }).ConfigureAwait(false);
        });

        //跳转历史数据页面
        public DelegateCommand Historycmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserContent = historyDataUserControl;
                });
            }).ConfigureAwait(false);
        });


        //跳转设备管理页面
        public DelegateCommand Devicecmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserContent = deviceManageUserControl;
                });
            }).ConfigureAwait(false);
        });

        //跳转用户管理页面
        public DelegateCommand UserCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserManage userManage = new UserManage();
                    userManage.ShowDialog();
                });
            }).ConfigureAwait(false);
        });

        //退出程序指令
        public DelegateCommand ExitCmd => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确认退出系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                });
            }).ConfigureAwait(false);
        });
    }
}
    

