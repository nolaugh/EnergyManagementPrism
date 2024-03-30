using EnergyManagementPrism.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnergyManagementPrism.Views
{
    /// <summary>
    /// DeviceManageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceManageUserControl : UserControl
    {
        public DeviceManageUserControl()
        {
            InitializeComponent();
            DeviceManageUserControlViewModel deviceManageUserControlViewModel=new DeviceManageUserControlViewModel();
            this.DataContext = deviceManageUserControlViewModel;
        }

        private void dgv_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as DeviceManageUserControlViewModel;
            var selectedDevice = vm.DeviceSelectItem;
            if (selectedDevice != null)
            {
                vm.TID = vm.DeviceSelectItem.ID;
                vm.TName=vm.DeviceSelectItem.Name;
                vm.TVolotage = vm.DeviceSelectItem.Volotage;
                vm.TElectricit = vm.DeviceSelectItem.Electricit;
                vm.TPower = vm.DeviceSelectItem.Power;
                vm.TAddress=vm.DeviceSelectItem.Address;
            }

        }
    }
}
