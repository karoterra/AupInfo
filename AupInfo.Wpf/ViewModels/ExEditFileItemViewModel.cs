using System.IO;
using System.Reactive.Linq;
using AupInfo.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFileItemViewModel : ItemViewModelBase
    {
        public ReadOnlyReactivePropertySlim<string> FilePath { get; }
        public ReadOnlyReactivePropertySlim<string> FileName { get; }
        public ReadOnlyReactivePropertySlim<string> FileType { get; }
        public ReadOnlyReactivePropertySlim<string> Location { get; }

        private readonly ExEditFile file;

        public ExEditFileItemViewModel(ExEditFile file)
        {
            this.file = file.AddTo(disposables);

            FilePath = this.file.FilePath
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            FileName = FilePath
                .Select(x => Path.GetFileName(x))
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            FileType = this.file.FileType
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            Location = this.file.Location
                .Select(x => x switch
                {
                    ExEditFileLocation.Path => "パス",
                    ExEditFileLocation.Project => "プロジェクト",
                    ExEditFileLocation.NotFound => "見つかりません",
                    _ => string.Empty
                })
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
        }
    }
}
