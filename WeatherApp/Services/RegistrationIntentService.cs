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
using Android.Preferences;
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;
using Android.Util;


namespace WeatherApp.Services
{
    [Service]
    public class RegistrationIntentService : IntentService
    {
        private const string TAG = "RegIntentService";

        public RegistrationIntentService () : base(TAG)
        {
        }

        protected override void OnHandleIntent (Intent intent)
        {
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

            try
            {
                // In the (unlikely) event that multiple refresh operations occur simultaneously,
                // ensure that they are processed sequentially.

                // Initially this call goes out to the network to retrieve the token, subsequent calls
                // are local.
                InstanceID instanceID = InstanceID.GetInstance(this);
                var token = instanceID.GetToken(GetString(Resource.String.gcm_defaultSenderId),
                GoogleCloudMessaging.InstanceIdScope, null);
                SendRegistrationToServer(token);

                // You should store a boolean that indicates whether the generated token has been
                // sent to your server. If the boolean is false, send the token to your server,
                // otherwise your server should have already received the token.
                sharedPreferences.Edit().PutBoolean(MainActivity.SENT_TOKEN_TO_SERVER, true).Apply();

            }
            catch (Exception e)
            {
                Log.Debug(TAG, "Failed to complete token refresh", e);

                // If an exception happens while fetching the new token or updating our registration data
                // on a third-party server, this ensures that we'll attempt the update at a later time.
                sharedPreferences.Edit().PutBoolean(MainActivity.SENT_TOKEN_TO_SERVER, false).Apply();
            }
        }

        /**
         * Normally, you would want to persist the registration to third-party servers. Because we do
         * not have a server, and are faking it with a website, you'll want to log the token instead.
         * That way you can see the value in logcat, and note it for future use in the website.
         *
         * @param token The new token.
         */
        private void SendRegistrationToServer (String token)
        {
            Log.Info(TAG, "GCM Registration Token: " + token);
        }
    }
}