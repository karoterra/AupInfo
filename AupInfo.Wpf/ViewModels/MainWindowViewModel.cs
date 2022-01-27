using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AupInfo.Core;
using AupInfo.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Prism.Events;
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

        public ReadOnlyReactivePropertySlim<Thickness> ContentMargin { get; }
        public ReadOnlyReactivePropertySlim<ScrollBarVisibility> HorizontalScrollBarVisibility { get; }
        public ReadOnlyReactivePropertySlim<ScrollBarVisibility> VerticalScrollBarVisibility { get; }

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
        private readonly MainSnackbarEvent snackbarEvent;
        private readonly AupFile aup;

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator ea, AupFile aup)
        {
            this.regionManager = regionManager;
            snackbarEvent = ea.GetEvent<MainSnackbarEvent>();
            this.aup = aup;

            snackbarEvent.Subscribe(EnqueueSnackbarMessage);

            Title = new ReactivePropertySlim<string>("AupInfo").AddTo(disposables);
            Subtitle = new ReactivePropertySlim<string>("Title").AddTo(disposables);
            FilePath = this.aup.FilePath.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            Filename = this.aup.FileName.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            IsMenuOpen = new ReactivePropertySlim<bool>(false).AddTo(disposables);
            IsModeless = new ReactivePropertySlim<bool>(false).AddTo(disposables);
            PanelItems = new ReactiveCollection<PanelItemViewModel>().AddTo(disposables);
            SelectedIndex = new ReactivePropertySlim<int>(-1).AddTo(disposables);
            SelectedItem = new ReactivePropertySlim<PanelItemViewModel?>(null).AddTo(disposables);

            ContentMargin = SelectedItem
                .Select(x => x?.Margin ?? new Thickness(8))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
            HorizontalScrollBarVisibility = SelectedItem
                .Select(x => x?.HorizontalScrollBarVisibility ?? ScrollBarVisibility.Auto)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
            VerticalScrollBarVisibility = SelectedItem
                .Select(x => x?.VerticalScrollBarVisibility ?? ScrollBarVisibility.Auto)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);

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

            PanelItems.AddRangeOnScheduler(GetPanelItems());
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

        private void EnqueueSnackbarMessage(Action<ISnackbarMessageQueue> action)
        {
            action.Invoke(SnackbarMessageQueue);
        }

        private IEnumerable<PanelItemViewModel> GetPanelItems()
        {
            yield return new("エディットハンドル", "", nameof(EditHandlePanel), SelectedItemClicked);
            yield return new("フィルタプラグイン", "", nameof(FilterProjectPanel), SelectedItemClicked)
            {
                Margin = new Thickness(8, 8, 0, 8),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("シーン", "拡張編集", nameof(ExEditScenePanel), SelectedItemClicked)
            {
                Margin = new(8, 8, 0, 8),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("ファイル", "拡張編集", nameof(ExEditFilePanel), SelectedItemClicked)
            {
                Margin = new(0, 8, 0, 0),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("フォント", "拡張編集", nameof(ExEditFontPanel), SelectedItemClicked)
            {
                Margin = new(8, 8, 0, 0),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("スクリプト", "拡張編集", nameof(ExEditScriptPanel), SelectedItemClicked)
            {
                Margin = new(0, 8, 0, 0),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("図形", "拡張編集", nameof(ExEditFigurePanel), SelectedItemClicked)
            {
                Margin = new(0, 8, 0, 0),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("トランジション", "拡張編集", nameof(ExEditTransitionPanel), SelectedItemClicked)
            {
                Margin = new(0, 8, 0, 0),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("ファイル", "PSDToolKit", nameof(PsdToolKitPanel), SelectedItemClicked)
            {
                Margin = new(8, 8, 0, 8),
                RequireHorizontalScroll = false,
                RequireVerticalScroll = false,
            };
            yield return new("AupInfo について", "その他", nameof(AboutPanel), SelectedItemClicked);
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
                    snackbarEvent.Unsubscribe(EnqueueSnackbarMessage);
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
