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
using Android.Accounts;

/**
 * Manages "Authentication" to Sunshine's backend service.  The SyncAdapter framework
 * requires an authenticator object, so syncing to a service that doesn't need authentication
 * typically means creating a stub authenticator like this one.
 * This code is copied directly, in its entirety, from
 * http://developer.android.com/training/sync-adapters/creating-authenticator.html
 * Which is a pretty handy reference when creating your own syncadapters.  Just sayin'.
 */
namespace WeatherApp.Sync
{
    [Service]
    public class SunshineAuthenticator : AbstractAccountAuthenticator
    {
        public SunshineAuthenticator (Context context) :
            base(context)
        {


        }

        public override Bundle EditProperties (
                AccountAuthenticatorResponse r, String s)
        {
            throw new NotImplementedException();
        }


        public override Bundle AddAccount (AccountAuthenticatorResponse r, String s, String s2, String[] strings, Bundle bundle)
        {
            return null;
        }

        public override Bundle ConfirmCredentials (AccountAuthenticatorResponse r, Account account, Bundle bundle)
        {
            return null;
        }

        public override Bundle GetAuthToken (AccountAuthenticatorResponse r, Account account, String s, Bundle bundle)
        {
            throw new NotImplementedException();
        }

        public override string GetAuthTokenLabel (String s)
        {
            throw new NotImplementedException();
        }

        public override Bundle UpdateCredentials (AccountAuthenticatorResponse r, Account account, String s, Bundle bundle)
        {
            throw new NotImplementedException();
        }

        public override Bundle HasFeatures (AccountAuthenticatorResponse r, Account account, String[] strings)
        {
            throw new NotImplementedException();
        }

    }
}