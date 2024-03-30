using EnergyManagementPrism.PublicClass;
using MySql.Data.MySqlClient;
using Prism.Commands;
using Prism.Mvvm;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnergyManagementPrism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "智能能耗监测管理系统";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
       
        //MainWindowViewModel入口
        public MainWindowViewModel()
        {
           
        }

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
