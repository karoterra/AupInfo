using System.Reactive.Disposables;
using System.Reactive.Linq;
using AupInfo.Core;
using Karoterra.AupDotNet.Extensions;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class EditHandlePanelViewModel : BindableBase, IDestructible
    {
        public ReactivePropertySlim<string?> EditFilename { get; }
        public ReactivePropertySlim<string?> OutputFilename { get; }
        public ReactivePropertySlim<string?> ProjectFilename { get; }
        public ReactivePropertySlim<int?> Width { get; }
        public ReactivePropertySlim<int?> Height { get; }
        public ReactivePropertySlim<string?> VideoRate { get; }
        public ReactivePropertySlim<int?> AudioRate { get; }
        public ReactivePropertySlim<int?> AudioCh { get; }
        public ReactivePropertySlim<int?> FrameNum { get; }
        public ReactivePropertySlim<string?> TimeLength { get; }
        public ReactivePropertySlim<string?> Flag { get; }
        public ReactivePropertySlim<int?> SelectedFrameStart { get; }
        public ReactivePropertySlim<int?> SelectedFrameEnd { get; }
        public ReactivePropertySlim<int?> VideoDecodeBit { get; }
        public ReactivePropertySlim<string?> VideoDecodeFormat { get; }

        private readonly CompositeDisposable disposables = new();
        private readonly AupFile aup;

        public EditHandlePanelViewModel(AupFile _aup)
        {
            aup = _aup;

            EditFilename = new ReactivePropertySlim<string?>("1920x1080_30fps_44100Hz.exedit").AddTo(disposables);
            OutputFilename = new ReactivePropertySlim<string?>(@"C:\path\to\video.mp4").AddTo(disposables);
            ProjectFilename = new ReactivePropertySlim<string?>(@"C:\path\to\project.aup").AddTo(disposables);
            Width = new ReactivePropertySlim<int?>(1920).AddTo(disposables);
            Height = new ReactivePropertySlim<int?>(1080).AddTo(disposables);
            VideoRate = new ReactivePropertySlim<string?>("30").AddTo(disposables);
            AudioRate = new ReactivePropertySlim<int?>(44100).AddTo(disposables);
            AudioCh = new ReactivePropertySlim<int?>(2).AddTo(disposables);
            FrameNum = new ReactivePropertySlim<int?>(120).AddTo(disposables);
            TimeLength = new ReactivePropertySlim<string?>("00:00:00.000").AddTo(disposables);
            Flag = new ReactivePropertySlim<string?>(null).AddTo(disposables);
            SelectedFrameStart = new ReactivePropertySlim<int?>(0).AddTo(disposables);
            SelectedFrameEnd = new ReactivePropertySlim<int?>(120).AddTo(disposables);
            VideoDecodeBit = new ReactivePropertySlim<int?>(48).AddTo(disposables);
            VideoDecodeFormat = new ReactivePropertySlim<string?>("XXXX").AddTo(disposables);

            aup.EditHandle.Subscribe(edit =>
            {
                EditFilename.Value = edit?.EditFilename;
                OutputFilename.Value = edit?.OutputFilename;
                ProjectFilename.Value = edit?.ProjectFilename;
                Width.Value = edit?.Width;
                Height.Value = edit?.Height;
                AudioRate.Value = edit?.AudioRate;
                AudioCh.Value = edit?.AudioCh;
                Flag.Value = edit?.Flag.ToString("X8");
                SelectedFrameStart.Value = edit?.SelectedFrameStart;
                SelectedFrameEnd.Value = edit?.SelectedFrameEnd;
                VideoDecodeBit.Value = edit?.VideoDecodeBit;
                VideoDecodeFormat.Value = edit?.VideoDecodeFormat.ToBytes().ToCleanSjisString();
                if (edit == null)
                {
                    VideoRate.Value = null;
                    FrameNum.Value = null;
                    TimeLength.Value = null;
                }
                else
                {
                    double fps = (double)edit.VideoRate / edit.VideoScale;
                    VideoRate.Value = fps.ToString();
                    FrameNum.Value = edit.Frames.Count;
                    var ts = new TimeSpan((long)(edit.Frames.Count / fps * 10000000));
                    TimeLength.Value = ts.ToString($@"{(ts.Days > 0 ? @"d\." : "")}hh\:mm\:ss\.fff");
                }
            }).AddTo(disposables);
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
