using System.Reactive.Disposables;
using AupInfo.Core;
using AupInfo.Wpf.Services;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class FilterProjectPanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<FilterProjectItemViewModel> Filters { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly AupFile aup;
        private readonly SaveFileDialogService saveFileDialogService;

        public FilterProjectPanelViewModel(AupFile _aup, SaveFileDialogService _saveFileDialogService)
        {
            aup = _aup;
            saveFileDialogService = _saveFileDialogService;

            Filters = aup.FilterProjects
                .ToReadOnlyReactiveCollection(f => new FilterProjectItemViewModel(f, aup.FilePath.Value!, saveFileDialogService))
                .AddTo(disposables);
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
