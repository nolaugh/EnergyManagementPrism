using LiveChartsCore;
using Prism.Mvvm;
using Prism.Commands;
using System.Threading.Tasks;
using System.Windows;
using EnergyManagementPrism.Views;
using System.Threading;

namespace EnergyManagementPrism.ViewModels
{
    public class MonitorUserControlViewModel:BindableBase
    {
        private int _permissions;
        public int Permissions
        {
            get { return _permissions; }
            set { SetProperty(ref _permissions, value); }
        }

        public ISeries[] Series { get; set; }

        public MonitorUserControlViewModel()
        {


        }

        public DelegateCommand connectManage => new DelegateCommand(async () =>
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                   ConnectManageWindow connectManageWindow = new ConnectManageWindow();
                   connectManageWindow.ShowDialog();
                });
            }).ConfigureAwait(false);
        });
       
    }
}
