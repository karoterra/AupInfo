using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace AupInfo.Wpf
{
    internal class DataContextDisposeAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            (AssociatedObject?.DataContext as IDisposable)?.Dispose();
        }
    }
}
