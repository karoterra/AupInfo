using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using AupInfo.Core;
using AupInfo.Wpf.Services;
using Karoterra.AupDotNet.ExEdit;
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
        private readonly AupFile aup;
        private readonly SaveFileDialogService saveFileDialogService;
        private readonly ObservableCollection<ExEditScene> scenes;
        private readonly MainSnackbarEvent snackbarEvent;

        public ExEditScenePanelViewModel(AupFile aup, SaveFileDialogService sfds, IEventAggregator ea)
        {
            this.aup = aup;
            saveFileDialogService = sfds;
            snackbarEvent = ea.GetEvent<MainSnackbarEvent>();

            scenes = new();
            Items = scenes
                .ToReadOnlyReactiveCollection(s => new ExEditSceneItemViewModel(
                    s, this.aup.FilePath.Value!, saveFileDialogService, snackbarEvent))
                .AddTo(disposables);
            this.aup.ExEdit
                .Subscribe(exedit => Update(exedit))
                .AddTo(disposables);
        }

        private void Update(ExEditProject? exedit)
        {
            scenes.Clear();
            if (exedit == null) return;
            scenes.AddRange(exedit.Scenes
                .Select(s => new ExEditScene(aup.EditHandle.Value!, exedit, s)));
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
