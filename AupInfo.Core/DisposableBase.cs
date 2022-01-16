using System.Reactive.Disposables;

namespace AupInfo.Core
{
    public abstract class DisposableBase : IDisposable
    {
        protected readonly CompositeDisposable disposables = new();

        #region IDisposable

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

        #endregion
    }
}
