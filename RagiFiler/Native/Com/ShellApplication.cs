using System;
using System.Collections.Generic;
using System.IO;

namespace RagiFiler.Native.Com
{
    // TODO: .net core 3.1 で dynamic で com object のメンバにアクセスできない・・・修正されたら dynamic に切り替える
    // あれ・・・？ https://github.com/dotnet/corefx/issues/32630
    class ShellApplication : IDisposable
    {
        private ComObject _instance;

        public ShellApplication()
        {
            _instance = ComObject.CreateFromProgID("shell.application");
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public Folder2 NameSpace(string path)
        {
            return new Folder2(_instance.InvokeMethod("NameSpace", path).AsComObject());
        }

        public IEnumerable<string> GetFolderVerbsTest2(string path)
        {
            string dir = Path.GetDirectoryName(path);
            using var folder = NameSpace(dir);
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
                    using var verb = verbs.Item(j);

                    if (string.IsNullOrEmpty(verb.Name?.Trim()))
                    {
                        continue;
                    }

                    yield return verb.Name;
                }
            }
        }
    }
}
