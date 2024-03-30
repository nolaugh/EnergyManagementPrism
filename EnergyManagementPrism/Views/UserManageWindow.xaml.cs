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

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// UserManage.xaml 的交互逻辑
    /// </summary>
    public partial class UserManage : Window
    {
        public UserManage()
        {
            InitializeComponent();
            UserManageWindowViewModel userManageWindowViewModel = new UserManageWindowViewModel();
            this.DataContext = userManageWindowViewModel;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as UserManageWindowViewModel;
            var selectedDevice = vm.UserSelectItems;
            if (selectedDevice != null)
            {
                vm.TAccount = vm.UserSelectItems.Account;
                vm.TPassword = vm.UserSelectItems.Password;
                vm.TPermissions = vm.UserSelectItems.Permissions; 
            }
        }
    }
}
