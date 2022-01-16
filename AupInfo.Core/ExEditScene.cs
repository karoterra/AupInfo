using System.Reactive.Linq;
using System.Text;
using Karoterra.AupDotNet;
using Karoterra.AupDotNet.ExEdit;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public class ExEditScene : DisposableBase
    {
        public ReactivePropertySlim<int> SceneIndex { get; }
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<int> Width { get; }
        public ReactivePropertySlim<int> Height { get; }
        public ReactivePropertySlim<int> FrameNum { get; }
        public ReadOnlyReactivePropertySlim<string> TimeLength { get; }

        private readonly EditHandle editHandle;
        private readonly ExEditProject exedit;
        private readonly Scene scene;

        public ExEditScene(EditHandle editHandle, ExEditProject exedit, Scene scene)
        {
            this.editHandle = editHandle;
            this.exedit = exedit;
            this.scene = scene;

            SceneIndex = new ReactivePropertySlim<int>((int)scene.SceneIndex).AddTo(disposables);
            Name = new ReactivePropertySlim<string>().AddTo(disposables);
            Width = new ReactivePropertySlim<int>().AddTo(disposables);
            Height = new ReactivePropertySlim<int>().AddTo(disposables);
            FrameNum = new ReactivePropertySlim<int>().AddTo(disposables);

            Name.Value = !string.IsNullOrEmpty(scene.Name) ? scene.Name : scene.SceneIndex == 0 ? "Root" : $"Scene {scene.SceneIndex}";
            if (exedit.EditingScene == scene.SceneIndex)
            {
                Width.Value = editHandle.Width;
                Height.Value = editHandle.Height;
            }
            else
            {
                Width.Value = (int)scene.Width;
                Height.Value = (int)scene.Height;
            }
            FrameNum.Value = exedit.EditingScene == scene.SceneIndex ? editHandle.Frames.Count : (int)scene.MaxFrame;
            double fps = (double)editHandle.VideoRate / editHandle.VideoScale;
            TimeLength = FrameNum
                .Select(x => BuildTimeLength(x))
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
        }

        public void SaveExo(string path)
        {
            var exo = exedit.ExportObject((int)scene.SceneIndex, editHandle);
            using FileStream fs = File.Create(path);
            using StreamWriter sw = new(fs, Encoding.GetEncoding("shift_jis"));
            sw.NewLine = "\r\n";
            exo.Write(sw);
        }

        private string BuildTimeLength(int frameNum)
        {
            double fps = (double)editHandle.VideoRate / editHandle.VideoScale;
            TimeSpan ts = new((long)(frameNum / fps * 10000000));
            return ts.ToString($@"{(ts.Days > 0 ? @"d\." : "")}hh\:mm\:ss\.fff");
        }
    }
}
