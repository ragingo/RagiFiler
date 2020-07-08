using System;

namespace RagiFiler.Native.Com.Shell
{
    class FolderItems : IComCollection<FolderItem>, IDisposable
    {
        private readonly ComObject _instance;

        public FolderItems(ComObject instance)
        {
            _instance = instance;
        }

        public int Count => _instance.GetProperty<int>("Count");

        public FolderItem Item(int index)
        {
            return new FolderItem(_instance.InvokeMethod("Item", index).AsComObject());
        }

        public void Dispose()
        {
            _instance.Dispose();
        }
    }
}
