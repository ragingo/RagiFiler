﻿using System;

namespace RagiFiler.Native.Com.Shell
{
    // TODO: .net core 3.1 で dynamic で com object のメンバにアクセスできない・・・修正されたら dynamic に切り替える
    // あれ・・・？ https://github.com/dotnet/corefx/issues/32630
    class ShellApplication : IShellDispatch, IDisposable
    {
        private readonly ComObject _instance;

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
            return new Folder2(_instance.InvokeMethod("NameSpace", path).AsComObject());
        }
    }
}
