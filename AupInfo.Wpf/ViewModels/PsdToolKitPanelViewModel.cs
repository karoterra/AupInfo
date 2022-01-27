using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class PsdToolKitPanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<PsdToolKitItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly PsdToolKitRepository repository;
        private readonly ObservableCollection<PsdToolKitItem> psdItems = new();

        public PsdToolKitPanelViewModel(PsdToolKitRepository psd)
        {
            repository = psd;

            Items = psdItems
                .ToReadOnlyReactiveCollection(x => new PsdToolKitItemViewModel(x))
                .AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            psdItems.Clear();
            psdItems.AddRange(repository.GetPsdToolKitItems());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
