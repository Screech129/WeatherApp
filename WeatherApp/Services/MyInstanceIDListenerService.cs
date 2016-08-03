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
using Android.Gms.Gcm.Iid;

namespace WeatherApp.Services
{
    [Service(Exported = false)]
    [IntentFilter(new string[] {"com.google.android.gms.iid.InstanceID"})]
    public class MyInstanceIDListenerService : InstanceIDListenerService
    {
        private const string TAG = "MyInstanceIDLS";

   /**
    * Called if InstanceID token is updated. This may occur if the security of
    * the previous token had been compromised. This call is initiated by the
    * InstanceID provider.
    */
   
    public override void OnTokenRefresh ()
        {
            // Fetch updated Instance ID token.
            Intent intent = new Intent(this, typeof(RegistrationIntentService));
            StartService(intent);
        }



    }
}