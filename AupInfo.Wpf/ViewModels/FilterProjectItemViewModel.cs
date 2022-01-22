using System.IO;
using AupInfo.Wpf.Services;
using Karoterra.AupDotNet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class FilterProjectItemViewModel : ItemViewModelBase
    {
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<int> Size { get; }

        public AsyncReactiveCommand SaveButtonClick { get; }

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
    }
}
