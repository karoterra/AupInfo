using System.Text;
using System.Windows;
using AupInfo.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;

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
            Container.Resolve<IModuleManager>().LoadModule<Core.CoreModule>();
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<EditHandlePanel>();
            containerRegistry.RegisterForNavigation<FilterProjectPanel>();
            containerRegistry.RegisterForNavigation<ExEditScenePanel>();
            containerRegistry.RegisterForNavigation<ExEditFilePanel>();
            containerRegistry.RegisterForNavigation<ExEditFontPanel>();
            containerRegistry.RegisterForNavigation<ExEditScriptPanel>();
            containerRegistry.RegisterForNavigation<PsdToolKitPanel>();

            containerRegistry.RegisterSingleton<Services.SaveFileDialogService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Core.CoreModule>(InitializationMode.OnDemand);
        }
    }
}
