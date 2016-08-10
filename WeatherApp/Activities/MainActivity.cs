using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Util;
using System.Net;
using System.IO;
using System.Text;
using Android.Preferences;
using WeatherApp.Sync;
using Android.Gms.Common;
using WeatherApp.Services;
using Android.Support.V7.App;

namespace WeatherApp
{
    [Activity(Label = "WeatherApp", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ForecastFragment.Callback
    {
        string location = "";
        private const string DETAILFRAGMENT_TAG = "DFTAG";
        private static int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;
        public const string SENT_TOKEN_TO_SERVER = "sentTokenToServer";
        bool twoPane;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            location = Utility.GetPreferredLocation(this);

            SetContentView(Resource.Layout.Main);
            var toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            if (FindViewById(Resource.Id.weather_detail_container) != null)
            {
                twoPane = true;
                if (bundle == null)
                {
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.weather_detail_container,
                        new DetailFragment(), DETAILFRAGMENT_TAG)
                        .Commit();

                }
            }
            else
            {
                twoPane = false;
                SupportActionBar.Elevation = 0f;
            }

            ForecastFragment forecastFragment = FragmentManager.FindFragmentById<ForecastFragment>(Resource.Id.fragment_forecast);
            forecastFragment.setUseTodayLayout(!twoPane);

            SunshineSyncAdapter.InitializeSyncAdapter(this);

            if (CheckPlayServices())
            {
                ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
                bool sentToken = sharedPreferences.GetBoolean(SENT_TOKEN_TO_SERVER, false);
                if (!sentToken)
                {
                    Intent intent = new Intent(this, typeof(RegistrationIntentService));
                    StartService(intent);
                }
            }
        }

        public override bool OnCreateOptionsMenu (IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;

        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                StartActivity(new Intent(this, typeof(SettingsActivity)));
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnResume ()
        {
            base.OnResume();
            if (Utility.GetPreferredLocation(this) != location)
            {
                ForecastFragment ff = FragmentManager.FindFragmentById<ForecastFragment>(Resource.Id.fragment_forecast);
                if (ff != null)
                {
                    ff.OnLocationChanged();
                }

                DetailFragment df = (DetailFragment)SupportFragmentManager.FindFragmentByTag(DETAILFRAGMENT_TAG);
                if (df != null)
                {
                    df.OnLocationChanged(location);
                }
                location = Utility.GetPreferredLocation(this);
            }

        }

        public void OnItemSelected (Android.Net.Uri dateUri)
        {

            if (twoPane)
            {
                Bundle args = new Bundle();
                args.PutParcelable(DetailFragment.DETAIL_URI, dateUri);

                DetailFragment fragment = new DetailFragment();
                fragment.Arguments = args;

                SupportFragmentManager.BeginTransaction()
                         .Replace(Resource.Id.weather_detail_container, fragment, DETAILFRAGMENT_TAG)
                         .Commit();
            }
            else
            {
                Intent intent = new Intent(this, typeof(DetailActivity))
                             .SetData(dateUri);
                StartActivity(intent);
            }
        }


        private bool CheckPlayServices ()
        {
            GoogleApiAvailability apiAvailability = GoogleApiAvailability.Instance;
            int resultCode = apiAvailability.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (apiAvailability.IsUserResolvableError(resultCode))
                {
                    apiAvailability.GetErrorDialog(this, resultCode,
                    PLAY_SERVICES_RESOLUTION_REQUEST).Show();
                }
                else
                {
                    Log.Info("Play Services", "This device is not supported.");
                    Finish();
                }
                return false;
            }
            return true;
        }

    }
}


