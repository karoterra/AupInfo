using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AupInfo.Wpf.ViewModels
{
    public class PanelItemViewModel : ItemViewModelBase
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

        public PanelItemViewModel(
            string name, string category, string navigation,
            ICommand previewMouseLeftButtonUp)
        {
            Name = name;
            Category = category;
            Navigation = navigation;

            PreviewMouseLeftButtonUp = previewMouseLeftButtonUp;
        }
    }
}
