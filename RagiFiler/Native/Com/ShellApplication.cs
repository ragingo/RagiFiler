using System;
using System.Collections.Generic;
using System.IO;

namespace RagiFiler.Native.Com
{
    class Folder2 : IDisposable
    {
        private ComObject _shell;

        public Folder2(ComObject shell)
        {
            _shell = shell;
        }

        public ComObject Items()
        {
            return new ComObject(_shell.InvokeMethod("Items"));
        }

        public void Dispose()
        {
            _shell.Dispose();
        }
    }

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
            return new Folder2(new ComObject(_instance.InvokeMethod("NameSpace", path)));
        }

        public IEnumerable<string> GetFolderVerbsTest2(string path)
        {
            string dir = Path.GetDirectoryName(path);
            using var folder = NameSpace(dir);
            using var items = folder.Items();
            int itemCount = items.GetProperty<int>("Count");
            yield return "TODO: ";
        }

        public IEnumerable<string> GetFolderVerbsTest(string path)
        {
            string dir = Path.GetDirectoryName(path);

            using var folder = new ComObject(_instance.InvokeMethod("NameSpace", dir));
            using var items = new ComObject(folder.InvokeMethod("Items"));
            int itemsCount = items.GetProperty<int>("Count");

            for (int i = 0; i < itemsCount; i++)
            {
                using var item = new ComObject(items.InvokeMethod("Item", i));
                var itemPath = item.GetProperty<string>("Path");

                if (Path.GetFullPath(itemPath) != Path.GetFullPath(path))
                {
                    continue;
                }

                using var verbs = new ComObject(item.InvokeMethod("Verbs"));
                var verbsCount = verbs.GetProperty<int>("Count");

                for (int j = 0; j < verbsCount; j++)
                {
                    using var verb = new ComObject(verbs.InvokeMethod("Item", j));
                    var verbName = verb.GetProperty<string>("Name");
                    if (string.IsNullOrEmpty(verbName?.Trim()))
                    {
                        continue;
                    }
                    yield return verbName;
                }
            }

            // TODO: .net core 3.1 で dynamic で com object のメンバにアクセスできない・・・修正されたら dynamic に切り替える
            // あれ・・・？ https://github.com/dotnet/corefx/issues/32630
            //var folder = _instance.NameSpace(dir);
        }

    }
}
