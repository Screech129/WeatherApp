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

namespace WeatherApp.Sync
{
    [Service]
    [IntentFilter(new[] { "android.accounts.AccountAuthenticator" })]
    [MetaData("android.accounts.AccountAuthenticator", Resource= "@xml/authenticator")]
    public class SunshineAuthenticatorService : Service
    {
        private SunshineAuthenticator auth;

        public override void OnCreate ()
        {
            auth = new SunshineAuthenticator(this);
        }

        public override IBinder OnBind (Intent intent)
        {
            return auth.IBinder;
        }
    }
}