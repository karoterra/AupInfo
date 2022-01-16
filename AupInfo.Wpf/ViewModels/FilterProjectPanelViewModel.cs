using System.Reactive.Disposables;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class FilterProjectPanelViewModel : BindableBase, IDestructible
    {
        public ReactiveCollection<string> Filters { get; }

        private readonly CompositeDisposable disposables = new();

        public FilterProjectPanelViewModel()
        {
            Filters = new ReactiveCollection<string>().AddTo(disposables);

            Filters.Add("拡張編集");
            Filters.Add("ごちゃまぜドロップス");
            Filters.Add("PSDToolKit");
            Filters.Add("拡張編集RAMプレビュー");
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
