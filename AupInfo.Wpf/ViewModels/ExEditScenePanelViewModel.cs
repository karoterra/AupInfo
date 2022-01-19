using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using AupInfo.Wpf.Services;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditScenePanelViewModel : BindableBase, IDestructible
    {
        public ReadOnlyReactiveCollection<ExEditSceneItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly ExEditRepository repository;
        private readonly SaveFileDialogService saveFileDialogService;
        private readonly ObservableCollection<ExEditScene> scenes = new();
        private readonly MainSnackbarEvent snackbarEvent;

        public ExEditScenePanelViewModel(ExEditRepository exedit, SaveFileDialogService sfds, IEventAggregator ea)
        {
            repository = exedit;
            saveFileDialogService = sfds;
            snackbarEvent = ea.GetEvent<MainSnackbarEvent>();

            Items = scenes
                .ToReadOnlyReactiveCollection(s => new ExEditSceneItemViewModel(
                    s, repository.AupPath.Value!, saveFileDialogService, snackbarEvent))
                .AddTo(disposables);
            repository.Updated
                .Subscribe(Update)
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            scenes.Clear();
            scenes.AddRange(repository.GetScenes());
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
