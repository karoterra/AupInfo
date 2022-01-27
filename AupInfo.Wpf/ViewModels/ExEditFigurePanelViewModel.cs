using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditFigurePanelViewModel : BindableBase, IDestructible
    {
        public ReactiveCollection<ExEditFigure> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;

        public ExEditFigurePanelViewModel(ExEditRepository exedit)
        {
            repository = exedit;

            Items = new ReactiveCollection<ExEditFigure>().AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            Items.ClearOnScheduler();
            Items.AddRangeOnScheduler(repository.GetFigures());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
