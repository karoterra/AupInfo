using System.Reactive.Disposables;
using System.Reactive.Linq;
using AupInfo.Core;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class FrameStatusPanelViewModel : BindableBase, IDestructible
    {
        public ReactiveCollection<FrameStatusItemViewModel> Items { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly AupFile aup;

        public FrameStatusPanelViewModel(AupFile aupFile)
        {
            aup = aupFile;

            Items = new ReactiveCollection<FrameStatusItemViewModel>()
                .AddTo(disposables);

            aup.EditHandle
                .Subscribe(_ => Update())
                .AddTo(disposables);

            Update();
        }

        private void Update()
        {
            Items.ClearOnScheduler();
            if (aup.EditHandle.Value == null) return;

            var frames = aup.EditHandle.Value.Frames;
            for (int i = 0; i < frames.Count; i++)
            {
                Items.AddOnScheduler(new FrameStatusItemViewModel(i, frames[i]));
            }
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
