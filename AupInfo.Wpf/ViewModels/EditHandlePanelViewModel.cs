using System.Reactive.Disposables;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class EditHandlePanelViewModel : BindableBase, IDestructible
    {
        public ReactivePropertySlim<string> EditFilename { get; }
        public ReactivePropertySlim<string> OutputFilename { get; }
        public ReactivePropertySlim<string> ProjectFilename { get; }
        public ReactivePropertySlim<int> Width { get; }
        public ReactivePropertySlim<int> Height { get; }
        public ReactivePropertySlim<string> VideoRate { get; }
        public ReactivePropertySlim<int> AudioRate { get; }
        public ReactivePropertySlim<int> AudioCh { get; }
        public ReactivePropertySlim<int> FrameNum { get; }
        public ReactivePropertySlim<string> TimeLength { get; }

        private readonly CompositeDisposable disposables = new();

        public EditHandlePanelViewModel()
        {
            EditFilename = new ReactivePropertySlim<string>("1920x1080_30fps_44100Hz.exedit").AddTo(disposables);
            OutputFilename = new ReactivePropertySlim<string>(@"C:\path\to\video.mp4").AddTo(disposables);
            ProjectFilename = new ReactivePropertySlim<string>(@"C:\path\to\project.aup").AddTo(disposables);
            Width = new ReactivePropertySlim<int>(1920).AddTo(disposables);
            Height = new ReactivePropertySlim<int>(1080).AddTo(disposables);
            VideoRate = new ReactivePropertySlim<string>("30").AddTo(disposables);
            AudioRate = new ReactivePropertySlim<int>(44100).AddTo(disposables);
            AudioCh = new ReactivePropertySlim<int>(2).AddTo(disposables);
            FrameNum = new ReactivePropertySlim<int>(120).AddTo(disposables);
            TimeLength = new ReactivePropertySlim<string>("00:00:00.000").AddTo(disposables);
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
