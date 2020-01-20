using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace RagiFiler.Native.Com
{
    static class ComObjectExtensions
    {
        public static ComObject AsComObject(this object obj)
        {
            return new ComObject(obj);
        }

        public static ComObject<T> AsComObject<T>(this T obj)
        {
            return new ComObject<T>(obj);
        }
    }

    interface IComCollection<T>
    {
        int Count { get; }
        T Item(int index);
    }

    class ComObject : ComObject<object>
    {
        public ComObject(object instance) : base(instance)
        {
        }

        public static new ComObject CreateFromProgID(string id)
        {
            return new ComObject(Activator.CreateInstance(Type.GetTypeFromProgID(id)));
        }
    }

    class ComObject<T> : CriticalFinalizerObject, IDisposable
    {
        private T _instance;

        public ComObject(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            _instance = instance;
        }

        public static implicit operator ComObject<T>(T instance)
        {
            return new ComObject<T>(instance);
        }

        public T Get()
        {
            return _instance;
        }

        public static ComObject<T> CreateFromProgID(string id)
        {
            return new ComObject<T>((T)Activator.CreateInstance(Type.GetTypeFromProgID(id)));
        }

        public object InvokeMember(string name, BindingFlags flags = BindingFlags.Default, params object[] args)
        {
            return _instance.GetType().InvokeMember(name, flags, null, _instance, args, CultureInfo.InvariantCulture);
        }

        public TResult InvokeMember<TResult>(string name, BindingFlags flags = BindingFlags.Default, params object[] args)
        {
            return (TResult)_instance.GetType().InvokeMember(name, flags, null, _instance, args, CultureInfo.InvariantCulture);
        }

        public TResult GetProperty<TResult>(string name)
        {
            return InvokeMember<TResult>(name, BindingFlags.GetProperty);
        }

        public object InvokeMethod(string name, params object[] args)
        {
            return InvokeMember(name, BindingFlags.InvokeMethod, args);
        }

        public TResult InvokeMethod<TResult>(string name, params object[] args)
        {
            return InvokeMember<TResult>(name, BindingFlags.InvokeMethod, args);
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
                _instance = default;

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
