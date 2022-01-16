using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFontItemViewModel : BindableBase, IDisposable
    {
        public ReadOnlyReactivePropertySlim<string> Name { get; }
        public ReadOnlyReactivePropertySlim<bool> IsAvailable { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly FontInfo fontInfo;

        public ExEditFontItemViewModel(FontInfo fi)
        {
            fontInfo = fi.AddTo(disposables);

            Name = fontInfo.Name
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            IsAvailable = fontInfo.IsAvailable
                .ToReadOnlyReactivePropertySlim()
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
