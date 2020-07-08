using System;

namespace RagiFiler.Native.Com.Shell
{
    class FolderItem : IDisposable
    {
        private readonly ComObject _instance;

        public FolderItem(ComObject instance)
        {
            _instance = instance;
        }

        public string Path => System.IO.Path.GetFullPath(_instance.GetProperty<string>("Path"));

        public FolderItemVerbs Verbs()
        {
            return new FolderItemVerbs(_instance.InvokeMethod("Verbs").AsComObject());
        }

        public void Dispose()
        {
            _instance.Dispose();
        }
    }
}
