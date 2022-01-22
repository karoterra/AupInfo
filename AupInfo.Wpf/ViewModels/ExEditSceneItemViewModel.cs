using System.IO;
using AupInfo.Core;
using AupInfo.Wpf.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditSceneItemViewModel : ItemViewModelBase
    {
        public ReadOnlyReactivePropertySlim<string> Name { get; }
        public ReadOnlyReactivePropertySlim<int> Width { get; }
        public ReadOnlyReactivePropertySlim<int> Height { get; }
        public ReadOnlyReactivePropertySlim<int> FrameNum { get; }
        public ReadOnlyReactivePropertySlim<string> TimeLength { get; }

        public ReactiveCommand SaveButtonClick { get; }

        private ExEditScene scene;
        private readonly string aupFilePath;
        private readonly SaveFileDialogService saveFileDialogService;
        private readonly MainSnackbarEvent snackbarEvent;

        public ExEditSceneItemViewModel(ExEditScene scene, string aupFilePath, SaveFileDialogService sfds, MainSnackbarEvent mse)
        {
            this.scene = scene.AddTo(disposables);
            this.aupFilePath = aupFilePath;
            this.saveFileDialogService = sfds;
            snackbarEvent = mse;

            Name = scene.Name.ToReadOnlyReactivePropertySlim<string>().AddTo(disposables);
            Width = scene.Width.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            Height = scene.Height.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            FrameNum = scene.FrameNum.ToReadOnlyReactivePropertySlim().AddTo(disposables);
            TimeLength = scene.TimeLength.ToReadOnlyReactivePropertySlim<string>().AddTo(disposables);

            SaveButtonClick = new ReactiveCommand()
                .WithSubscribe(Export)
                .AddTo(disposables);
        }

        public void Export()
        {
            SaveFileDialogSetting setting = new()
            {
                FileName = $"{Path.GetFileNameWithoutExtension(aupFilePath)}_{scene.SceneIndex.Value}.exo",
                InitialDirectory = Path.GetDirectoryName(aupFilePath) ?? @"C:\",
                Filter = "オブジェクトファイル(*.exo)|*.exo|すべてのファイル(*.*)|*.*",
                Title = "オブジェクトファイルの保存先を選択してください",
            };
            if (saveFileDialogService.ShowDialog(setting) == true)
            {
                try
                {
                    scene.SaveExo(setting.FileName);
                    snackbarEvent.Publish(q => q.Enqueue($"オブジェクトファイルを \"{setting.FileName}\" に保存しました。"));
                }
                catch (UnauthorizedAccessException)
                {
                    snackbarEvent.Publish(q => q.Enqueue($"\"{setting.FileName}\" へのアクセス許可がありません。"));
                }
                catch (PathTooLongException)
                {
                    snackbarEvent.Publish(q => q.Enqueue($"パスが長すぎます。"));
                }
                catch (Exception e) when (
                    e is ArgumentException or
                    ArgumentNullException or
                    DirectoryNotFoundException or
                    NotSupportedException)
                {
                    snackbarEvent.Publish(q => q.Enqueue("有効な出力パスを指定してください。"));
                }
                catch (IOException)
                {
                    snackbarEvent.Publish(q => q.Enqueue("オブジェクトファイル保存中にIOエラーが発生しました。"));
                }
            }
        }
    }
}
