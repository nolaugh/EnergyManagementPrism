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
    /// ControlPolicyUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPolicyUserControl : UserControl
    {
        public ControlPolicyUserControl()
        {
            InitializeComponent();
            ControlPolicyUserControlViewModel viewModel = new ControlPolicyUserControlViewModel();
            this.DataContext = viewModel;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as ControlPolicyUserControlViewModel;
            var selectedDevice = vm.ControlSelectItem;
            if (selectedDevice != null)
            {
                vm.TDevice = vm.ControlSelectItem.Device;    
                vm.TPoint = vm.ControlSelectItem.Point;
                vm.TCondition = vm.ControlSelectItem.Condition;
                vm.TThreshold = vm.ControlSelectItem.Threshold;
                vm.TDo = vm.ControlSelectItem.Do;
                vm.TOpenClose = vm.ControlSelectItem.OpenClose;
            }
        }

        //暂定
        private void cmb_Sensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
