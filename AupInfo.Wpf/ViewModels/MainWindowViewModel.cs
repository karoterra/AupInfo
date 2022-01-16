using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using AupInfo.Core;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        public ReactivePropertySlim<string> Title { get; }
        public ReactivePropertySlim<string> Subtitle { get; }
        public ReadOnlyReactivePropertySlim<string?> FilePath { get; }
        public ReadOnlyReactivePropertySlim<string?> Filename { get; }

        public ReactivePropertySlim<bool> IsMenuOpen { get; }
        public ReactivePropertySlim<bool> IsModeless { get; }
        public ReadOnlyReactivePropertySlim<DrawerHostOpenMode> DrawerOpenMode { get; }

        public ReactiveCollection<PanelItemViewModel> PanelItems { get; }
        public ReactivePropertySlim<int> SelectedIndex { get; }
        public ReactivePropertySlim<PanelItemViewModel?> SelectedItem { get; }

        public ReactiveCommand Loaded { get; }
        public ReactiveCommand<MouseButtonEventArgs> SelectedItemClicked { get; }
        public ReactiveCommand<DragEventArgs> Drop { get; }
        public ReactiveCommand<DragEventArgs> DragOver { get; }

        public SnackbarMessageQueue SnackbarMessageQueue { get; } = new();

        private readonly CompositeDisposable disposables = new();
        private readonly IRegionManager regionManager;
        private readonly AupFile aup;

        public MainWindowViewModel(IRegionManager regionManager, AupFile aup)
        {
            this.regionManager = regionManager;
            this.aup = aup;

            Title = new ReactivePropertySlim<string>("AupInfo").AddTo(disposables);
            Subtitle = new ReactivePropertySlim<string>("Title").AddTo(disposables);
            FilePath = this.aup.FilePath.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            Filename = this.aup.FileName.ToReadOnlyReactivePropertySlim().AddTo(disposables);
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

            DragOver = new ReactiveCommand<DragEventArgs>()
                .WithSubscribe(e =>
                {
                    e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
                    e.Handled = true;
                })
                .AddTo(disposables);
            Drop = new ReactiveCommand<DragEventArgs>()
                .WithSubscribe(e =>
                {
                    if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

                    var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    OpenAup(files![0]);
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

        private void OpenAup(string path)
        {
            if (Directory.Exists(path))
            {
                SnackbarMessageQueue.Enqueue($"\"{path}\" はディレクトリです。");
                return;
            }

            try
            {
                aup.Open(path);
                SnackbarMessageQueue.Enqueue($"\"{path}\" を開きました。");
            }
            catch (FileNotFoundException)
            {
                SnackbarMessageQueue.Enqueue($"\"{path}\" が見つかりません。");
                aup.Close();
            }
            catch (UnauthorizedAccessException)
            {
                SnackbarMessageQueue.Enqueue($"\"{path}\" へのアクセス許可がありません。");
                aup.Close();
            }
            catch (PathTooLongException)
            {
                SnackbarMessageQueue.Enqueue("パスが長すぎます。");
                aup.Close();
            }
            catch (Exception e) when (
                e is ArgumentException or
                ArgumentNullException or
                DirectoryNotFoundException or
                NotSupportedException)
            {
                SnackbarMessageQueue.Enqueue("有効なパスを指定してください。");
                aup.Close();
            }
            catch (FileFormatException)
            {
                SnackbarMessageQueue.Enqueue($"\"{path}\" はAviUtlプロジェクトファイルでないか破損している可能性があります。");
                aup.Close();
            }
            catch (EndOfStreamException)
            {
                SnackbarMessageQueue.Enqueue($"\"{path}\" はAviUtlプロジェクトファイルでないか破損している可能性があります。");
                aup.Close();
            }
            catch (IOException)
            {
                SnackbarMessageQueue.Enqueue("IOエラーが発生しました。");
                aup.Close();
            }
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
