using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace RagiFiler.Native.Com
{
    class ComObject : CriticalFinalizerObject, IDisposable
    {
        private object _instance;

        public ComObject(object instance)
        {
            _instance = instance;
        }

        public static ComObject CreateFromProgID(string id)
        {
            return new ComObject(Activator.CreateInstance(Type.GetTypeFromProgID(id)));
        }

        public object InvokeMember(string name, BindingFlags flags = BindingFlags.Default, params object[] args)
        {
            return _instance.GetType().InvokeMember(name, flags, null, _instance, args, CultureInfo.InvariantCulture);
        }

        public T InvokeMember<T>(string name, BindingFlags flags = BindingFlags.Default, params object[] args)
        {
            return (T)_instance.GetType().InvokeMember(name, flags, null, _instance, args, CultureInfo.InvariantCulture);
        }

        public T GetProperty<T>(string name)
        {
            return InvokeMember<T>(name, BindingFlags.GetProperty);
        }

        public object InvokeMethod(string name, params object[] args)
        {
            return InvokeMember(name, BindingFlags.InvokeMethod, args);
        }

        public T InvokeMethod<T>(string name, params object[] args)
        {
            return InvokeMember<T>(name, BindingFlags.InvokeMethod, args);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                Marshal.FinalReleaseComObject(_instance);
                // TODO: 大きなフィールドを null に設定します。
                _instance = null;

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ComObject()
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
