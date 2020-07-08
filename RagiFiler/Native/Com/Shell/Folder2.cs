using System;

namespace RagiFiler.Native.Com.Shell
{
    class Folder2 : IDisposable
    {
        private readonly ComObject _instance;

        public Folder2(ComObject instance)
        {
            _instance = instance;
        }

        public FolderItems Items()
        {
            return new FolderItems(_instance.InvokeMethod("Items").AsComObject());
        }

        public void Dispose()
        {
            _instance.Dispose();
        }
    }
}
