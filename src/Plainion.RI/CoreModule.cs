using Plainion.RI.Controls;
using Plainion.RI.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Plainion.RI
{
    class CoreModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.Dialogs, typeof(SelectFolderDialogView));

            regionManager.RegisterViewWithRegion(RegionNames.Controls, typeof(EditableTextBlockView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
