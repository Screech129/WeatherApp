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
using Android.Support.V4.App;
using Android.Support.V4.Util;
using WeatherApp.Services;
using Android.Support.V7.App;
using WeatherApp.Fragments;
using Uri = Android.Net.Uri;

namespace WeatherApp
{
    [Activity(Label = "WeatherApp", MainLauncher = true, Theme = "@style/AppTheme.Main")]
    public class MainActivity : AppCompatActivity, ForecastFragment.ICallback
    {
        string location = "";
        private const string DetailfragmentTag = "DFTAG";
        private static int _playServicesResolutionRequest = 9000;
        public const string SentTokenToServer = "sentTokenToServer";
        bool twoPane;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            location = Utility.GetPreferredLocation(this);
            var contentUri = Intent?.Data;
            SetContentView(Resource.Layout.Main);
            var toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            if (FindViewById(Resource.Id.weather_detail_container) != null)
            {
                twoPane = true;
                if (bundle == null)
                {
                    var fragment = new DetailFragment();
                    if (contentUri != null)
                    {
                        var args = new Bundle();
                        args.PutParcelable(DetailFragment.DetailUri,contentUri);
                        fragment.Arguments = args;
                    }
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.weather_detail_container,
                        fragment, DetailfragmentTag)
                        .Commit();

                }
            }
            else
            {
                twoPane = false;
                SupportActionBar.Elevation = 0f;
            }

            var forecastFragment = FragmentManager.FindFragmentById<ForecastFragment>(Resource.Id.fragment_forecast);
            forecastFragment.SetUseTodayLayout(!twoPane);

            if (contentUri != null)
            {
                forecastFragment.SetInitialSelectedDate(WeatherContractOpen.WeatherEntryOpen.GetDateFromUri(contentUri));
            }

            SunshineSyncAdapter.InitializeSyncAdapter(this);

            if (CheckPlayServices())
            {
                var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
                var sentToken = sharedPreferences.GetBoolean(SentTokenToServer, false);
                if (!sentToken)
                {
                    var intent = new Intent(this, typeof(RegistrationIntentService));
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
            var id = item.ItemId;
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
                var ff = FragmentManager.FindFragmentById<ForecastFragment>(Resource.Id.fragment_forecast);
                if (ff != null)
                {
                    ff.OnLocationChanged();
                }

                var df = (DetailFragment)SupportFragmentManager.FindFragmentByTag(DetailfragmentTag);
                if (df != null)
                {
                    df.OnLocationChanged(location);
                }
                location = Utility.GetPreferredLocation(this);
            }

        }

        public void OnItemSelected (Uri dateUri, ForecastAdapter.ForecastAdapterViewHolder vh)
        {

            if (twoPane)
            {
                var args = new Bundle();
                args.PutParcelable(DetailFragment.DetailUri, dateUri);

                var fragment = new DetailFragment();
                fragment.Arguments = args;

                SupportFragmentManager.BeginTransaction()
                         .Replace(Resource.Id.weather_detail_container, fragment, DetailfragmentTag)
                         .Commit();
            }
            else
            {
                var intent = new Intent(this, typeof(DetailActivity))
                             .SetData(dateUri);

                var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, new Android.Support.V4.Util.Pair(vh.IconView,GetString(Resource.String.detail_icon_transition_name)));
                ActivityCompat.StartActivity(this,intent,options.ToBundle());
            }
        }


        private bool CheckPlayServices ()
        {
            var apiAvailability = GoogleApiAvailability.Instance;
            var resultCode = apiAvailability.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (apiAvailability.IsUserResolvableError(resultCode))
                {
                    apiAvailability.GetErrorDialog(this, resultCode,
                    _playServicesResolutionRequest).Show();
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


