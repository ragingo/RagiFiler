﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.Media;

namespace RagiFiler.ViewModels.Components
{
    class DirectoryTreeViewItemViewModel : BindableBase
    {
        public WeakReference<DirectoryTreeViewItemViewModel> Parent { get; set; }
        public ObservableCollection<DirectoryTreeViewItemViewModel> Children { get; } = new ObservableCollection<DirectoryTreeViewItemViewModel>();
        public FileSystemInfo Item { get; set; }
        public bool IsDirectory { get { return Item is DirectoryInfo; } }
        public bool IsHiddenFile { get { return (Item.Attributes & FileAttributes.Hidden) > 0; } }
        public bool IsSystemFile { get { return (Item.Attributes & FileAttributes.System) > 0; } }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    SetProperty(ref _icon, ImageUtils.GetFileIcon(Item.FullName), nameof(Icon));
                }
                return _icon;
            }
        }

        public int Level
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }

                if (Parent.TryGetTarget(out var item))
                {
                    return item.Level + 1;
                }

                return 0;
            }
        }

        public DirectoryTreeViewItemViewModel(string path)
        {
            Item = new DirectoryInfo(path);
        }

        public DirectoryTreeViewItemViewModel(FileSystemInfo info)
        {
            Item = info;
        }

        public async Task LoadSubDirectories()
        {
            if (Children.Count > 0)
            {
                return;
            }

            await foreach (var info in IOUtils.LoadFileSystemInfosAsync(Item.FullName).OfType<DirectoryInfo>())
            {
                var item = new DirectoryTreeViewItemViewModel(info) { Parent = new WeakReference<DirectoryTreeViewItemViewModel>(this) };
                Children.Add(item);
            }
        }
    }
}