﻿using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using AupInfo.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AupInfo.Wpf.ViewModels
{
    public class PsdToolKitItemViewModel : ItemViewModelBase
    {
        public ReactivePropertySlim<string?> FileName { get; }
        public ReactivePropertySlim<string?> FilePath { get; }
        public ReadOnlyReactivePropertySlim<Visibility> IconVisibility { get; }
        public ReactivePropertySlim<int?> Tag { get; }
        public ReactivePropertySlim<ImageSource?> Thumbnail { get; }

        private readonly PsdToolKitItem item;

        public PsdToolKitItemViewModel(PsdToolKitItem item)
        {
            this.item = item.AddTo(disposables);

            FilePath = new ReactivePropertySlim<string?>(this.item.Image).AddTo(disposables);
            FileName = new ReactivePropertySlim<string?>(Path.GetFileName(FilePath.Value)).AddTo(disposables);
            IconVisibility = FilePath
                .Select(x => File.Exists(x) ? Visibility.Collapsed : Visibility.Visible)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
            Tag = new ReactivePropertySlim<int?>(this.item.Tag).AddTo(disposables);
            Thumbnail = new ReactivePropertySlim<ImageSource?>(this.item.Thumbnail == null ? null : ImageUtil.BitmapToBitmapSource(this.item.Thumbnail))
                .AddTo(disposables);
        }
    }
}
