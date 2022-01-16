using System.Reactive.Disposables;
using System.Windows.Input;
using Prism.Mvvm;

namespace AupInfo.Wpf.ViewModels
{
    public class PanelItemViewModel : BindableBase, IDisposable
    {
        public string Name { get; }
        public string Category { get; }
        public string Navigation { get; }

        public ICommand PreviewMouseLeftButtonUp { get; }

        private readonly CompositeDisposable disposables = new();

        public PanelItemViewModel(
            string name, string category, string navigation,
            ICommand previewMouseLeftButtonUp)
        {
            Name = name;
            Category = category;
            Navigation = navigation;

            PreviewMouseLeftButtonUp = previewMouseLeftButtonUp;
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
