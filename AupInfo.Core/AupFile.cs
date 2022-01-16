using System.Reactive.Disposables;
using System.Reactive.Linq;
using Karoterra.AupDotNet;
using Karoterra.AupDotNet.ExEdit;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public class AupFile : IDisposable
    {
        public ReadOnlyReactivePropertySlim<string?> FilePath { get; }
        public ReadOnlyReactivePropertySlim<string?> FileName { get; }

        public ReactivePropertySlim<EditHandle?> EditHandle { get; }
        public ReactiveCollection<FilterProject> FilterProjects { get; }
        public ReactivePropertySlim<ExEditProject?> ExEdit { get; }

        public ReactiveCommand Opened { get; }

        private readonly ReactivePropertySlim<string?> filepath;
        private AviUtlProject? aup = null;
        private readonly CompositeDisposable disposables = new();

        public AupFile()
        {
            filepath = new ReactivePropertySlim<string?>(null)
                .AddTo(disposables);

            FilePath = filepath.ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
            FileName = filepath
                .Select(x => Path.GetFileName(x))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);

            EditHandle = new ReactivePropertySlim<EditHandle?>(null)
                .AddTo(disposables);
            FilterProjects = new ReactiveCollection<FilterProject>()
                .AddTo(disposables);
            ExEdit = new ReactivePropertySlim<ExEditProject?>(null)
                .AddTo(disposables);

            Opened = new ReactiveCommand()
                .AddTo(disposables);
        }

        public void Open(string path)
        {
            aup = new(path);
            EditHandle.Value = aup.EditHandle;
            filepath.Value = path;

            FilterProjects.ClearOnScheduler();
            FilterProjects.AddRangeOnScheduler(aup.FilterProjects);

            var filter = aup.FilterProjects.FirstOrDefault(f => f.Name == "拡張編集");
            if (filter != null)
            {
                ExEdit.Value = new(filter.DumpData());
            }

            Opened.Execute();
        }

        public void Close()
        {
            aup = null;
            ExEdit.Value = null;
            EditHandle.Value = null;
            FilterProjects.ClearOnScheduler();
            filepath.Value = null;
        }

        public void Save(string? path = null)
        {
            if (aup == null) return;
            if (path == null)
                path = filepath.Value!;

            using var fs = File.Create(path);
            using BinaryWriter bw = new(fs);
            aup.Write(bw);
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
