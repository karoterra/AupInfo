using System.Text.Json;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Core
{
    public class PsdToolKitRepository : DisposableBase
    {
        public ReactiveCommand Updated { get; }

        private readonly AupFile aup;

        public PsdToolKitRepository(AupFile aup)
        {
            this.aup = aup;

            Updated = new ReactiveCommand().AddTo(disposables);
            this.aup.FilterProjects
                .CollectionChangedAsObservable()
                .Subscribe(_ => Updated.Execute())
                .AddTo(disposables);
        }

        public List<PsdToolKitItem> GetPsdToolKitItems()
        {
            var filter = aup.FilterProjects.FirstOrDefault(f => f.Name == "PSDToolKit");
            if (filter == null)
            {
                return new List<PsdToolKitItem>();
            }

            var data = filter.DumpData();
            if (data.Length <= 0 || data[0] != '[')
            {
                return new List<PsdToolKitItem>();
            }
            var items = JsonSerializer.Deserialize<List<PsdToolKitItem>>(data);
            return items ?? new List<PsdToolKitItem>();
        }
    }
}
