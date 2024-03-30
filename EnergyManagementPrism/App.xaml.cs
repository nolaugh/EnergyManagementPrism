using EnergyManagementPrism.Views;
using Prism.Ioc;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Threading;

namespace EnergyManagementPrism
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }  
    }
}
