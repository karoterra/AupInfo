using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public class FontInfo : DisposableBase
    {
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<bool> IsAvailable { get; }

        public FontInfo(string name, bool isAvailable)
        {
            Name = new ReactivePropertySlim<string>(name).AddTo(disposables);
            IsAvailable = new ReactivePropertySlim<bool>(isAvailable).AddTo(disposables);
        }
    }
}
