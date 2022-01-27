using System.Reactive.Linq;
using Karoterra.AupDotNet.ExEdit;
using Karoterra.AupDotNet.ExEdit.Effects;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public enum ExEditScriptKind
    {
        Anm,
        Obj,
        Scn,
        Cam,
        Tra,
    }

    public class ExEditScript : DisposableBase
    {
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<ExEditScriptKind> Kind { get; }
        public ReadOnlyReactivePropertySlim<string> FileName { get; }

        public ExEditScript(string name, ExEditScriptKind kind)
        {
            Name = new ReactivePropertySlim<string>(name).AddTo(disposables);
            Kind = new ReactivePropertySlim<ExEditScriptKind>(kind).AddTo(disposables);
            FileName = Name.CombineLatest(Kind, GetFileName)
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
        }

        private static string GetFileName(string name, ExEditScriptKind kind)
        {
            switch (kind)
            {
                case ExEditScriptKind.Anm when AnimationEffect.Defaults.Contains(name):
                    return "exedit.anm";
                case ExEditScriptKind.Obj when CustomObjectEffect.Defaults.Contains(name):
                    return "exedit.obj";
                case ExEditScriptKind.Scn when SceneChangeEffect.DefaultScripts.Contains(name):
                    return "exedit.scn";
                case ExEditScriptKind.Cam when CameraEffect.Defaults.Contains(name):
                    return "exedit.cam";
                case ExEditScriptKind.Tra when TrackbarScript.Defaults.Any(t => t.Name == name):
                    return "exedit.tra";
            }
            string ext = kind.ToString().ToLower();
            int index = name.IndexOf('@');
            if (index >= 0)
            {
                name = name[index..];
            }
            return $"{name}.{ext}";
        }

        public override bool Equals(object? obj)
        {
            return obj is ExEditScript script
                && Name.Value == script.Name.Value
                && Kind.Value == script.Kind.Value;
        }

        public override int GetHashCode()
        {
            return Name.Value.GetHashCode() ^ Kind.Value.GetHashCode();
        }
    }
}
