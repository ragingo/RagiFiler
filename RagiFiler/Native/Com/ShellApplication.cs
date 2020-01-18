using System;
using System.Collections.Generic;
using System.IO;

namespace RagiFiler.Native.Com
{
    class ShellApplication : IDisposable
    {
        private ComObject _instance;

        public ShellApplication()
        {
            _instance = ComObject.CreateFromProgID("shell.application");
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
                    yield return verbName;
                }
            }

            // TODO: .net core 3.1 で dynamic で com object のメンバにアクセスできない・・・修正されたら dynamic に切り替える
            // あれ・・・？ https://github.com/dotnet/corefx/issues/32630
            //var folder = _instance.NameSpace(dir);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                _instance.Dispose();
                // TODO: 大きなフィールドを null に設定します。
                _instance = null;

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ShellApplication()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
