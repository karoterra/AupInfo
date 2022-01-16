using Prism.Ioc;
using Prism.Modularity;

namespace AupInfo.Core
{
    public class CoreModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AupFile>();
        }
    }
}
