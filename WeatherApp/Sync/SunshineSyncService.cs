using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace WeatherApp.Sync
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "android.content.SyncAdapter"})]
    [MetaData("android.content.SyncAdapter", Resource="@xml/syncadapter")]
    public class SunshineSyncService : Service
    {
        private static object _syncAdapterLock = new object();
        private static SunshineSyncAdapter _sunshineSyncAdapter = null;

        public override void OnCreate ()
        {
            Log.Debug("SunshineSyncService", "OnCreate - SunshineSyncService");
            lock (_syncAdapterLock)
            {
                if(_sunshineSyncAdapter == null)
                {
                    _sunshineSyncAdapter = new SunshineSyncAdapter(this, true);
                }
            }
        }
       
        public override IBinder OnBind (Intent intent)
        {
            return _sunshineSyncAdapter.SyncAdapterBinder;
        }
    }
}