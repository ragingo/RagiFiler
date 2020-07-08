using System;

namespace RagiFiler.Native.Com.Shell
{
    class FolderItemVerbs : IComCollection<FolderItemVerb>, IDisposable
    {
        private readonly ComObject _instance;

        public FolderItemVerbs(ComObject instance)
        {
            _instance = instance;
        }

        public int Count => _instance.GetProperty<int>("Count");

        public FolderItemVerb Item(int index)
        {
            return new FolderItemVerb(_instance.InvokeMethod("Item", index).AsComObject());
        }

        public void Dispose()
        {
            _instance.Dispose();
        }
    }
}
