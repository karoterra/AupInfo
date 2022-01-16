using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Mvvm;

namespace AupInfo.Wpf.ViewModels
{
    public class PanelItemViewModel : BindableBase, IDisposable
    {
        public string Name { get; } = string.Empty;
        public string Category { get; } = string.Empty;
        public string Navigation { get; } = string.Empty;
        public Thickness Margin { get; set; } = new Thickness(8);
        public bool RequireHorizontalScroll { get; set; } = true;
        public bool RequireVerticalScroll { get; set; } = true;
        public ScrollBarVisibility HorizontalScrollBarVisibility
            => RequireHorizontalScroll ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        public ScrollBarVisibility VerticalScrollBarVisibility
            => RequireVerticalScroll ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;

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
