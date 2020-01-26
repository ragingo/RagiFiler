using System;

namespace RagiFiler.Native.Com
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

    class FolderItemVerb : IDisposable
    {
        private readonly ComObject _instance;

        public FolderItemVerb(ComObject instance)
        {
            _instance = instance;
        }

        public string Name => _instance.GetProperty<string>("Name");

        public void DoIt()
        {
            _instance.InvokeMethod("DoIt");
        }

        public void Dispose()
        {
            _instance.Dispose();
        }
    }
}
