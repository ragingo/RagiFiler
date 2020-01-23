using System.Collections.Generic;
using System.IO;
using Prism.Mvvm;
using RagiFiler.Native.Com;

namespace RagiFiler.Models
{
    class ContextMenuModel : BindableBase
    {
        public IEnumerable<FolderItemVerb> GetMenuItems(string path)
        {
            string dir = Path.GetDirectoryName(path);
            using var shell = new ShellApplication();
            using var folder = shell.NameSpace(dir);
            using var items = folder.Items();

            for (int i = 0; i < items.Count; i++)
            {
                using var item = items.Item(i);

                if (Path.GetFullPath(item.Path) != Path.GetFullPath(path))
                {
                    continue;
                }

                using var verbs = item.Verbs();

                for (int j = 0; j < verbs.Count; j++)
                {
#pragma warning disable CA2000 // スコープを失う前にオブジェクトを破棄
                    var verb = verbs.Item(j);
#pragma warning restore CA2000 // スコープを失う前にオブジェクトを破棄

                    if (string.IsNullOrEmpty(verb.Name?.Trim()))
                    {
                        continue;
                    }

                    yield return verb;
                }
            }
        }
    }
}
