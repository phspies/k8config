using Android.App;
using Android.Runtime;
using Microsoft.Maui;
using System;

namespace k8config_gui
{
    [Application]
    public class MainApplication : MauiApplication<Startup>
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }
    }
}