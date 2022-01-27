using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public enum ExEditFileLocation
    {
        Path,
        Project,
        NotFound,
    }

    public class ExEditFile : DisposableBase
    {
        public ReactivePropertySlim<string> FilePath { get; }
        public ReactivePropertySlim<string> FileType { get; }
        public ReactivePropertySlim<ExEditFileLocation> Location { get; }

        public ExEditFile(string path, string type, ExEditFileLocation location)
        {
            FilePath = new ReactivePropertySlim<string>(path).AddTo(disposables);
            FileType = new ReactivePropertySlim<string>(type).AddTo(disposables);
            Location = new ReactivePropertySlim<ExEditFileLocation>(location).AddTo(disposables);
        }

        public override bool Equals(object? obj)
        {
            return obj is ExEditFile file
                && FilePath.Value == file.FilePath.Value
                && FileType.Value == file.FileType.Value;
        }

        public override int GetHashCode()
        {
            return FilePath.Value.GetHashCode() ^ FileType.Value.GetHashCode();
        }
    }
}
