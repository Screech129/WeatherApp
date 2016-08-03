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
using Android.Gms.Gcm;
using Org.Json;
using Android.Util;
using Android.Graphics;
using Android.Support.V4.App;

namespace WeatherApp.Services
{
    [Service(Exported = false)]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        private string TAG = "MyGcmListenerService";

        private string EXTRA_DATA = "Bundle";
        private string EXTRA_WEATHER = "weather";
        private string EXTRA_LOCATION = "location";

        public int NOTIFICATION_ID = 1;

        public override void OnMessageReceived (string from, Bundle data)
        {
            // Time to unparcel the bundle!
            if (!data.IsEmpty)
            {
                // TODO: gcm_default sender ID comes from the API console
                var senderId = GetString(Resource.String.gcm_defaultSenderId);
                if (senderId.Length == 0)
                {
                    Toast.MakeText(this, "SenderID string needs to be set", ToastLength.Long).Show();
                }
                // Not a bad idea to check that the message is coming from your server.
                if ((senderId).Equals(from))
                {
                    // Process message and then post a notification of the received message.
                    try
                    {
                        //JSONObject jsonObject = new JSONObject(data.);
                        string weather = data.GetString(EXTRA_WEATHER);
                        string location = data.GetString(EXTRA_LOCATION);
                        string alert = string.Format(GetString(Resource.String.gcm_weather_alert), weather, location);
                        SendNotification(alert);
                    }
                    catch (JSONException e)
                    {
                        // JSON parsing failed, so we just let this message go, since GCM is not one
                        // of our critical features.
                    }
                }
                Log.Info(TAG, "Received: " + data.ToString());
                base.OnMessageReceived(from, data);
            }
        }

        private void SendNotification (string message)
        {
            NotificationManager mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            PendingIntent contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);

            // Notifications using both a large and a small icon (which yours should!) need the large
            // icon as a bitmap. So we need to create that here from the resource ID, and pass the
            // object along in our notification builder. Generally, you want to use the app icon as the
            // small icon, so that users understand what app is triggering this notification.
            Bitmap largeIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.art_storm);
            NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(this)
                            .SetSmallIcon(Resource.Drawable.art_clear)
                            .SetLargeIcon(largeIcon)
                            .SetContentTitle("Weather Alert!")
                            .SetStyle(new NotificationCompat.BigTextStyle().BigText(message))
                            .SetContentText(message)
                            .SetPriority(NotificationCompat.PriorityHigh);
            mBuilder.SetContentIntent(contentIntent);
            mNotificationManager.Notify(NOTIFICATION_ID, mBuilder.Build());
        }
    }
}