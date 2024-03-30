using EnergyManagementPrism.ViewModels;
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
using System.Windows.Threading;

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// HomeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HomeWindow : Window
    {
        public  HomeWindow(HomeWindowViewModel homeWindowViewModel)
        {
            InitializeComponent();

            this.DataContext = homeWindowViewModel;

            UpdateTimeAsync();
        }

        //系统时间更新方法
        private async void UpdateTimeAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        txt_Time.Text = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
                    });
                     Task.Delay(1000);
                }
            }).ConfigureAwait(false);  
        }

        //窗体加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;//还原窗口
            this.WindowStyle = System.Windows.WindowStyle.None; //仅工作区可见，不显示标题栏和边框
            this.ResizeMode = System.Windows.ResizeMode.NoResize;//不显示最大化和最小化按钮
            this.Left = 0.0;//设置左边距为0
            this.Top = 0.0;//设置顶边距为0
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;//设置为屏幕宽度
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;//设置为屏幕高度 
        }

        //窗体关闭事件
        private void Window_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
