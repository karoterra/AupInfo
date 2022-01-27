using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditScriptPanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<ExEditScriptItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;
        private readonly ObservableCollection<ExEditScript> scripts = new();

        public ExEditScriptPanelViewModel(ExEditRepository exedit)
        {
            repository = exedit;

            Items = scripts
                .ToReadOnlyReactiveCollection(s => new ExEditScriptItemViewModel(s))
                .AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            scripts.Clear();
            scripts.AddRange(repository.GetScripts());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
