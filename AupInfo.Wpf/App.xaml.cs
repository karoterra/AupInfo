using System.Text;
using System.Windows;
using Prism.Ioc;
using AupInfo.Wpf.Views;

namespace AupInfo.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<EditHandlePanel>();
            containerRegistry.RegisterForNavigation<FilterProjectPanel>();
        }
    }
}
