using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFilePanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<ExEditFileItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;
        private readonly ObservableCollection<ExEditFile> files = new();

        public ExEditFilePanelViewModel(ExEditRepository exedit)
        {
            repository = exedit;

            Items = files
                .ToReadOnlyReactiveCollection(f => new ExEditFileItemViewModel(f))
                .AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            files.Clear();
            files.AddRange(repository.GetFiles());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
