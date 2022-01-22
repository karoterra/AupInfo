using AupInfo.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFontItemViewModel : ItemViewModelBase
    {
        public ReadOnlyReactivePropertySlim<string> Name { get; }
        public ReadOnlyReactivePropertySlim<bool> IsAvailable { get; }

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
    }
}
