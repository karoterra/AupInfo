using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reflection;
using Octokit;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class AboutPanelViewModel : BindableBase, IDestructible
    {
        public ReactivePropertySlim<Version> Version { get; }
        public ReactivePropertySlim<string?> Copyright { get; }
        public ReactivePropertySlim<string> UpdateInfo { get; }

        public AsyncReactiveCommand CheckUpdateCommand { get; }
        public ReactiveCommand OpenGitHubCommand { get; }
        public ReactiveCommand OpenReleasesCommand { get; }
        public ReactiveCommand OpenCreditsCommand { get; }

        private readonly CompositeDisposable disposables = new();

        public AboutPanelViewModel()
        {
            var assm = Assembly.GetExecutingAssembly();
            var assmName = assm.GetName();
            var path = assm.Location;
            var info = FileVersionInfo.GetVersionInfo(path);

            Version = new ReactivePropertySlim<Version>(assmName.Version ?? new(0, 0, 0)).AddTo(disposables);
            Copyright = new ReactivePropertySlim<string?>(info.LegalCopyright).AddTo(disposables);
            UpdateInfo = new ReactivePropertySlim<string>().AddTo(disposables);

            CheckUpdateCommand = new AsyncReactiveCommand()
                .WithSubscribe(CheckUpdate)
                .AddTo(disposables);
            OpenGitHubCommand = new ReactiveCommand()
                .WithSubscribe(() => Link.OpenInBrowser("https://github.com/karoterra/AupInfo"))
                .AddTo(disposables);
            OpenReleasesCommand = new ReactiveCommand()
                .WithSubscribe(() => Link.OpenInBrowser("https://github.com/karoterra/AupInfo/releases"))
                .AddTo(disposables);
            OpenCreditsCommand = new ReactiveCommand()
                .WithSubscribe(() => Link.OpenInBrowser("https://github.com/karoterra/AupInfo/blob/main/CREDITS.md"))
                .AddTo(disposables);
        }

        private async Task CheckUpdate()
        {
            UpdateInfo.Value = "更新を確認中";

            Version latest;
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("AupInfo"));
                var releases = await client.Repository.Release.GetAll("karoterra", "AupInfo");
                if (releases.Count == 0)
                {
                    UpdateInfo.Value = "利用できるアップデートはありません";
                    return;
                }

                latest = new(releases[0].TagName[1..]);
            }
            catch (Exception)
            {
                UpdateInfo.Value = "更新情報の取得に失敗しました";
                return;
            }

            if (Version.Value < latest)
            {
                UpdateInfo.Value = $"バージョン {latest} を利用できます";
                return;
            }
            UpdateInfo.Value = "AupInfo は最新の状態です";
        }

        public void Destroy()
        {
            disposables.Dispose();
        }
    }
}
