using EnergyManagementPrism.Model;
using EnergyManagementPrism.PublicClass;
using EnergyManagementPrism.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySQLHelper sqlHelper = new MySQLHelper();
        LoginModel loginModel = new LoginModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        //窗口加载事件
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

        //登录功能
        private  void btn_Login_Click(object sender, RoutedEventArgs e)
        {
             Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try {
                        loginModel.Account = txt_Account.Text.ToString().Trim();
                        loginModel.Password = txt_Password.Password.Trim();
                        string query = "SELECT * FROM tb_login WHERE Account ='" + loginModel.Account + "' AND Password ='" + loginModel.Password + "'";
                        MySqlParameter[] parameters ={
                            new MySqlParameter("@Account", loginModel.Account),
                            new MySqlParameter("@Password",loginModel.Password)
                        };
                        DataTable loginTable = sqlHelper.ExecuteQuery(query, parameters);
                        string account = loginTable.Rows[0][1].ToString();
                        string password = loginTable.Rows[0][2].ToString();
                        int  permissions = Convert.ToInt16(loginTable.Rows[0][3]);
                        if (account == loginModel.Account&& password==loginModel.Password)
                        {
                            if (permissions == 1)
                            {
                                HandyControl.Controls.MessageBox.Info("登录成功！", "提示");
                                this.Hide();
                                HomeWindowViewModel homeWindowViewModel = new HomeWindowViewModel(permissions);
                                HomeWindow homeWindow = new HomeWindow(homeWindowViewModel);
                                MonitorUserControlViewModel monitorUserControlViewModel = new MonitorUserControlViewModel();
                                homeWindow.ShowDialog();
                            }
                            if (permissions == 2)
                            {
                                HandyControl.Controls.MessageBox.Info("登录成功！", "提示");
                                this.Hide();
                                HomeWindowViewModel homeWindowViewModel = new HomeWindowViewModel(permissions);
                                HomeWindow homeWindow = new HomeWindow(homeWindowViewModel);
                                MonitorUserControlViewModel monitorUserControlViewModel = new MonitorUserControlViewModel();
                                homeWindow.ShowDialog();
                            }
                        }
                        else
                        {
                            HandyControl.Controls.MessageBox.Error("登录失败！请检查账户信息！", "提示");
                        }
                    }
                    catch{ 
                        HandyControl.Controls.MessageBox.Error("登录失败！请检查账户信息！","提示");
                    }                                     
                });
            }).ConfigureAwait(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
