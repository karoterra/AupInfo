using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFontPanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<ExEditFontItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;
        private readonly ObservableCollection<FontInfo> fonts = new();

        public ExEditFontPanelViewModel(ExEditRepository exedit)
        {
            repository = exedit;

            Items = fonts
                .ToReadOnlyReactiveCollection(f => new ExEditFontItemViewModel(f))
                .AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            fonts.Clear();
            fonts.AddRange(repository.GetFontInfos());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
