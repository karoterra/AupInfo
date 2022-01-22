using System.Reactive.Disposables;
using Prism.Mvvm;

namespace AupInfo.Wpf.ViewModels
{
    public abstract class ItemViewModelBase : BindableBase, IDisposable
    {
        protected readonly CompositeDisposable disposables = new();
        protected bool disposedValue;

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
    }
}
