using System.Reactive.Linq;
using AupInfo.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class ExEditScriptItemViewModel : ItemViewModelBase
    {
        public ReadOnlyReactivePropertySlim<string> Name { get; }
        public ReadOnlyReactivePropertySlim<string> Kind { get; }
        public ReadOnlyReactivePropertySlim<string> FileName { get; }

        private readonly ExEditScript script;

        public ExEditScriptItemViewModel(ExEditScript script)
        {
            this.script = script.AddTo(disposables);

            Name = this.script.Name
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            Kind = this.script.Kind
                .Select(x => x.ToString().ToUpper())
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
            FileName = this.script.FileName
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(disposables);
        }
    }
}
