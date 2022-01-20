using System.Reactive.Disposables;
using System.Reactive.Linq;
using AupInfo.Core;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditScriptItemViewModel : BindableBase, IDisposable
    {
        public ReadOnlyReactivePropertySlim<string> Name { get; }
        public ReadOnlyReactivePropertySlim<string> Kind { get; }
        public ReadOnlyReactivePropertySlim<string> FileName { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditScript script;

        public ExEditScriptItemViewModel(ExEditScript script)
        {
            this.script = script.AddTo(disposables);

            Name = this.script.Name
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            Kind = this.script.Kind
                .Select(x => x.ToString().ToUpper())
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            FileName = this.script.FileName
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
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
