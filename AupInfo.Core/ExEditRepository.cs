using System.Drawing.Text;
using System.Text.RegularExpressions;
using Karoterra.AupDotNet;
using Karoterra.AupDotNet.ExEdit;
using Karoterra.AupDotNet.ExEdit.Effects;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public class ExEditRepository : DisposableBase
    {
        public ReadOnlyReactivePropertySlim<string?> AupPath { get; }

        public ReactiveCommand Updated { get; }

        private readonly AupFile aup;
        private ExEditProject? exedit = null;
        private static readonly HashSet<string> installedFonts = new();
        private static readonly Regex regex = new Regex(@"<s\d*,([^,>]+)(,[BI]*)?>");

        public ExEditRepository(AupFile aup)
        {
            this.aup = aup;

            AupPath = this.aup.FilePath
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);

            Updated = new ReactiveCommand().AddTo(disposables);

            this.aup.FilterProjects
                .CollectionChangedAsObservable()
                .Subscribe(_ => Update())
                .AddTo(disposables);

            Update();
        }

        public List<ExEditScene> GetScenes()
        {
            if (exedit == null)
            {
                return new List<ExEditScene>();
            }

            EditHandle editHandle = aup.EditHandle.Value!;
            var scenes = exedit.Scenes
                .Select(s => new ExEditScene(editHandle, exedit, s))
                .ToList();
            return scenes;
        }

        public List<FontInfo> GetFontInfos()
        {
            if (exedit == null)
            {
                return new List<FontInfo>();
            }

            HashSet<string> fonts = new();
            foreach (var obj in exedit.Objects)
            {
                if (obj.Chain) continue;
                if (obj.Effects[0] is TextEffect t)
                {
                    if (!string.IsNullOrEmpty(t.Font))
                    {
                        fonts.Add(t.Font);
                    }
                    foreach (Match m in regex.Matches(t.Text))
                    {
                        fonts.Add(m.Groups[1].Value);
                    }
                }
            }

            var fontInfos = fonts
                .Select(f => new FontInfo(f, installedFonts.Contains(f)))
                .ToList();
            return fontInfos;
        }

        private void Update()
        {
            var filter = aup.FilterProjects
                .FirstOrDefault(f => f.Name == "拡張編集");
            if (filter == null)
            {
                exedit = null;
                return;
            }

            exedit = new(filter.DumpData());
            Updated.Execute();
        }

        static ExEditRepository()
        {
            InstalledFontCollection ifc = new();
            foreach (var ff in ifc.Families)
            {
                installedFonts.Add(ff.Name);
            }
        }
    }
}
