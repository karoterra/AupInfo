using System.Reactive.Disposables;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditTransitionPanelViewModel : BindableBase, IDestructible
    {
        public ReactiveCollection<ExEditTransition> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;

        public ExEditTransitionPanelViewModel(ExEditRepository exedit)
        {
            repository = exedit;

            Items = new ReactiveCollection<ExEditTransition>().AddTo(disposables);

            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            Items.ClearOnScheduler();
            Items.AddRangeOnScheduler(repository.GetTransitions());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
