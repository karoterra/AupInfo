using System.IO;
using System.Reactive.Disposables;
using AupInfo.Wpf.Services;
using Karoterra.AupDotNet;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class FilterProjectItemViewModel : BindableBase, IDisposable
    {
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<int> Size { get; }

        public AsyncReactiveCommand SaveButtonClick { get; }

        private readonly CompositeDisposable disposables = new();

        private readonly FilterProject filter;
        private readonly string aupFilePath;
        private readonly SaveFileDialogService saveFileDialogService;

        public FilterProjectItemViewModel(FilterProject filter, string aupFilePath, SaveFileDialogService saveFileDialogService)
        {
            this.filter = filter;
            this.aupFilePath = aupFilePath;
            this.saveFileDialogService = saveFileDialogService;

            Name = new ReactivePropertySlim<string>(filter.Name).AddTo(disposables);
            Size = new ReactivePropertySlim<int>(filter.DumpData().Length).AddTo(disposables);

            SaveButtonClick = new AsyncReactiveCommand()
                .WithSubscribe(SaveAsync)
                .AddTo(disposables);
        }

        private async Task SaveAsync()
        {
            SaveFileDialogSetting setting = new()
            {
                FileName = $"{Path.GetFileNameWithoutExtension(aupFilePath)}_{filter.Name}.dat",
                InitialDirectory = Path.GetDirectoryName(aupFilePath) ?? @"C:\",
                Filter = "バイナリファイル(*.dat)|*.dat|すべてのファイル(*.*)|*.*",
                Title = "フィルタプラグインデータの保存先を選択してください",
            };
            if (saveFileDialogService.ShowDialog(setting) == true)
            {
                await File.WriteAllBytesAsync(setting.FileName, filter.DumpData());
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
