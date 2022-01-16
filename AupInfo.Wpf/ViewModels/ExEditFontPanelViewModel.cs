using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using AupInfo.Wpf.Repositories;
using Karoterra.AupDotNet.ExEdit;
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
        private readonly AupFile aup;
        private readonly FontInfoRepository fontInfoRepository;
        private readonly ObservableCollection<FontInfo> fonts = new();

        public ExEditFontPanelViewModel(AupFile aup, FontInfoRepository fir)
        {
            this.aup = aup;
            fontInfoRepository = fir;

            Items = fonts
                .ToReadOnlyReactiveCollection(f => new ExEditFontItemViewModel(f))
                .AddTo(disposables);

            this.aup.ExEdit
                .Subscribe(exedit => Update(exedit));
        }

        private void Update(ExEditProject? exedit)
        {
            fonts.Clear();
            if (exedit == null) return;
            fonts.AddRange(fontInfoRepository.GetAllFontInfo(exedit.Objects));
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
