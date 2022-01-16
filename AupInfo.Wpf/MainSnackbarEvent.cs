using MaterialDesignThemes.Wpf;
using Prism.Events;

namespace AupInfo.Wpf
{
    public class MainSnackbarEvent : PubSubEvent<Action<ISnackbarMessageQueue>>
    {
    }
}
