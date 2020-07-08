using System;

namespace RagiFiler.Native.Com.Shell
{
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
