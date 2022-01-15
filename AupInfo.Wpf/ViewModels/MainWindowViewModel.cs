using System.Reactive.Disposables;
using System.Reactive.Linq;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Input;

namespace AupInfo.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        public ReactivePropertySlim<string> Title { get; }
        public ReactivePropertySlim<string> Subtitle { get; }

        public ReactivePropertySlim<bool> IsMenuOpen { get; }
        public ReactivePropertySlim<bool> IsModeless { get; }
        public ReadOnlyReactivePropertySlim<DrawerHostOpenMode> DrawerOpenMode { get; }

        public ReactiveCollection<PanelItemViewModel> PanelItems { get; }
        public ReactivePropertySlim<int> SelectedIndex { get; }
        public ReactivePropertySlim<PanelItemViewModel?> SelectedItem { get; }

        public ReactiveCommand Loaded { get; }
        public ReactiveCommand<MouseButtonEventArgs> SelectedItemClicked { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly IRegionManager regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            Title = new ReactivePropertySlim<string>("AupInfo").AddTo(disposables);
            Subtitle = new ReactivePropertySlim<string>("Title").AddTo(disposables);
            IsMenuOpen = new ReactivePropertySlim<bool>(false).AddTo(disposables);
            IsModeless = new ReactivePropertySlim<bool>(false).AddTo(disposables);
            PanelItems = new ReactiveCollection<PanelItemViewModel>().AddTo(disposables);
            SelectedIndex = new ReactivePropertySlim<int>(-1).AddTo(disposables);
            SelectedItem = new ReactivePropertySlim<PanelItemViewModel?>(null).AddTo(disposables);

            DrawerOpenMode = IsModeless
                .Select(x => x ? DrawerHostOpenMode.Standard : DrawerHostOpenMode.Modal)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);

            Loaded = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    SelectedIndex.Value = 0;
                })
                .AddTo(disposables);

            SelectedItemClicked = new ReactiveCommand<MouseButtonEventArgs>()
                .WithSubscribe(e =>
                {
                    if (!IsModeless.Value)
                        IsMenuOpen.Value = false;
                })
                .AddTo(disposables);

            SelectedItem.Subscribe(item =>
            {
                if (!string.IsNullOrEmpty(item?.Navigation))
                {
                    this.regionManager.RequestNavigate("ContentRegion", item.Navigation);
                    Subtitle.Value = string.IsNullOrEmpty(item.Category)
                        ? item.Name
                        : $"{item.Category} - {item.Name}";
                    if (!IsModeless.Value)
                        IsMenuOpen.Value = false;
                }
            }).AddTo(disposables);

            PanelItems.Add(new("エディットハンドル", "", "EditHandlePanel", SelectedItemClicked));
            PanelItems.Add(new("フィルタプラグイン", "", "FilterProjectPanel", SelectedItemClicked));
            PanelItems.Add(new("シーン", "拡張編集", "", SelectedItemClicked));
            PanelItems.Add(new("フォント", "拡張編集", "", SelectedItemClicked));
            PanelItems.Add(new("ファイル", "PSDToolKit", "", SelectedItemClicked));
        }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var region in regionManager.Regions)
                    {
                        region.RemoveAll();
                    }
                    disposables.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
