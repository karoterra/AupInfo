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

        private static string? GetFileFromScript(ScriptFileEffect script)
        {
            if (script.Params == null) return null;

            var file = script.Params.GetValueOrDefault("file");
            if (file == null || file.Length < 2) return null;
            return file[1..^1].Replace(@"\\", @"\");
        }

        public List<ExEditFile> GetFiles()
        {
            if (exedit == null)
            {
                return new List<ExEditFile>();
            }

            string projectDir = Path.GetDirectoryName(aup.FilePath.Value) ?? string.Empty;
            HashSet<ExEditFile> files = new();
            foreach (var obj in exedit.Objects)
            {
                if (obj.Chain) continue;
                foreach (var effect in obj.Effects)
                {
                    (string? path, string type) = effect switch
                    {
                        VideoFileEffect video => (video.Filename, video.Type.Name),
                        ImageFileEffect image => (image.Filename, image.Type.Name),
                        AudioFileEffect audio => (audio.Filename, audio.Type.Name),
                        WaveformEffect waveform => (waveform.Filename, waveform.Type.Name),
                        ShadowEffect shadow => (shadow.Filename, shadow.Type.Name),
                        BorderEffect border => (border.Filename, border.Type.Name),
                        VideoCompositionEffect videoComp => (videoComp.Filename, videoComp.Type.Name),
                        ImageCompositionEffect imageComp => (imageComp.Filename, imageComp.Type.Name),
                        FigureEffect figure => (figure.Filename, figure.Type.Name),
                        MaskEffect mask => (mask.Filename, mask.Type.Name),
                        DisplacementEffect displacement => (displacement.Filename, displacement.Type.Name),
                        PartialFilterEffect partialFilter => (partialFilter.Filename, partialFilter.Type.Name),
                        ScriptFileEffect script => (GetFileFromScript(script), script.Type.Name),
                        _ => (null, string.Empty),
                    };
                    if (!string.IsNullOrEmpty(path))
                    {
                        ExEditFileLocation location = File.Exists(path)
                            ? ExEditFileLocation.Path
                            : File.Exists(Path.Combine(projectDir, Path.GetFileName(path)))
                                ? ExEditFileLocation.Project : ExEditFileLocation.NotFound;
                        files.Add(new ExEditFile(path, type, location));
                    }
                }
            }

            return files.ToList();
        }

        public List<ExEditScript> GetScripts()
        {
            if (exedit == null)
            {
                return new List<ExEditScript>();
            }

            HashSet<ExEditScript> scripts = new();
            foreach (var obj in exedit.Objects)
            {
                if (obj.Chain) continue;
                foreach (var effect in obj.Effects)
                {
                    ExEditScriptKind kind = ExEditScriptKind.Anm;
                    string name = string.Empty;
                    switch (effect)
                    {
                        case AnimationEffect anm:
                            kind = ExEditScriptKind.Anm;
                            name = string.IsNullOrEmpty(anm.Name) ? AnimationEffect.Defaults[anm.ScriptId] : anm.Name;
                            break;
                        case CustomObjectEffect coe:
                            kind = ExEditScriptKind.Obj;
                            name = string.IsNullOrEmpty(coe.Name) ? CustomObjectEffect.Defaults[coe.ScriptId] : coe.Name;
                            break;
                        case CameraEffect cam:
                            kind = ExEditScriptKind.Cam;
                            name = string.IsNullOrEmpty(cam.Name) ? CameraEffect.Defaults[cam.ScriptId] : cam.Name;
                            break;
                        case SceneChangeEffect scn when scn.Params != null:
                            kind = ExEditScriptKind.Scn;
                            name = scn.Name;
                            break;
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        scripts.Add(new ExEditScript(name, kind));
                    }
                    foreach (var track in effect.Trackbars)
                    {
                        if (track.Type == TrackbarType.Script)
                        {
                            name = exedit.TrackbarScripts[track.ScriptIndex].Name;
                            scripts.Add(new ExEditScript(name, ExEditScriptKind.Tra));
                        }
                    }
                }
            }

            return scripts.ToList();
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
