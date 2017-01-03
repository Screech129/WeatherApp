

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Android.Preferences;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Org.Json;
using Android.Database;
using WeatherApp.Sync;
using Android.Support.V7.Widget;
using Com.Bumptech.Glide.Manager;

namespace WeatherApp
{
    public class ForecastFragment : Fragment, LoaderManager.ILoaderCallbacks,
        ISharedPreferencesOnSharedPreferenceChangeListener
    {

        ForecastAdapter forecastAdapter;
        private const string LOG_TAG = "ForecastAdapter";
        private RecyclerView recyclerView;
        private int position = RecyclerView.NoPosition;
        private bool _useTodayLayout;
        private static string SELECTED_KEY = "selected_position";
        private const int URL_LOADER = 0;


        private string[] FORECAST_COLUMNS =
        {
            // In this case the id needs to be fully qualified with a table name, since
            // the content provider joins the location & weather tables in the background
            // (both have an _id column)
            // On the one hand, that's annoying.  On the other, you can search the weather table
            // using the location set by the user, which is only in the Location table.
            // So the convenience is worth it.
            WeatherContractOpen.WeatherEntryOpen.TABLE_NAME + "." + WeatherContractOpen.WeatherEntryOpen._ID,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP,
            WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id,
            WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT,
            WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG
        };

        // These indices are tied to FORECAST_COLUMNS.  If FORECAST_COLUMNS changes, these
        // must change.
        public const int COL_WEATHER_ID = 0;
        public const int COL_WEATHER_DATE = 1;
        public const int COL_WEATHER_DESC = 2;
        public const int COL_WEATHER_MAX_TEMP = 3;
        public const int COL_WEATHER_MIN_TEMP = 4;
        public const int COL_LOCATION_SETTING = 5;
        public const int COL_WEATHER_CONDITION_ID = 6;
        public const int COL_COORD_LAT = 7;
        public const int COL_COORD_LONG = 8;


        public interface Callback
        {
            void OnItemSelected (Android.Net.Uri dateUri);
        }

        public ForecastFragment ()
        {

        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        public override void OnResume ()
        {
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .RegisterOnSharedPreferenceChangeListener(this);
            base.OnResume();
        }

        public override void OnPause ()
        {
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .UnregisterOnSharedPreferenceChangeListener(this);
            base.OnPause();
        }

        public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.forecastfragment, menu);
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_viewLocation)
            {
                OpenPreferredLocationInMap();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_main, container, false);
            System.Diagnostics.Debug.WriteLine("On CreateView called Forecast Fragment");
            try
            {

                recyclerView = (RecyclerView)rootView.FindViewById(Resource.Id.recyclerview_forecast);

                var emptyView = rootView.FindViewById(Resource.Id.recyclerview_forecast_empty);
                recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                recyclerView.HasFixedSize = true;
                forecastAdapter = new ForecastAdapter(Activity, emptyView);
                forecastAdapter.ItemClick += OnClick;
                recyclerView.SetAdapter(forecastAdapter);


                if (savedInstanceState != null && savedInstanceState.ContainsKey(SELECTED_KEY))
                {
                    position = savedInstanceState.GetInt(SELECTED_KEY);
                }

                forecastAdapter.SetUseTodayLayout(_useTodayLayout);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Log.WriteLine(LogPriority.Error, LOG_TAG, ex.ToString());
                throw;
            }

            return rootView;
        }


        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            LoaderManager.InitLoader(URL_LOADER, null, this);
            base.OnActivityCreated(savedInstanceState);
        }

        public void OnLocationChanged ()
        {
            LoaderManager.RestartLoader(URL_LOADER, null, this);
        }

        private void OpenPreferredLocationInMap ()
        {
            ICursor c = forecastAdapter.GetCursor();
            if (null != c)
            {
                c.MoveToPosition(0);
                String posLat = c.GetString(COL_COORD_LAT);
                String posLong = c.GetString(COL_COORD_LONG);
                Android.Net.Uri geoLocation = Android.Net.Uri.Parse("geo:" + posLat + "," + posLong);

                var mapIntent = new Intent(Intent.ActionView, geoLocation);
                if (mapIntent.ResolveActivity(Activity.PackageManager) != null)
                {
                    StartActivity(mapIntent);
                }
                else
                {
                    Toast.MakeText(Activity, "No Map App found", ToastLength.Long).Show();
                    Log.Debug("Main Activity", "Couldn't call " + geoLocation.ToString() + ", No Maps App");
                }

            }

            //Alternative way of calling the activity but not a fully implicint intent
            //			var geoLocation = Android.Net.Uri.Parse("http://maps.google.com/maps/place/"+zipCode);
            //
            //			var mapIntent = new Intent(Intent.ActionView,geoLocation);
            //			mapIntent.SetClassName ("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            //			try
            //			{
            //				StartActivity (mapIntent);
            //
            //			}
            //			catch(ActivityNotFoundException ex)
            //			{
            //				try
            //				{
            //					Intent unrestrictedIntent = new Intent(Intent.ActionView, geoLocation);
            //					StartActivity(unrestrictedIntent);
            //				}
            //				catch(ActivityNotFoundException innerEx)
            //				{
            //					Toast.MakeText(this, "Please install a maps application", ToastLength.Long).Show();
            //				}
            //			}
        }

        public override void OnSaveInstanceState (Bundle outState)
        {
            if (position != RecyclerView.NoPosition)
            {
                outState.PutInt(SELECTED_KEY, position);
            }
            base.OnSaveInstanceState(outState);
        }

        public Loader OnCreateLoader (int id, Bundle args)
        {
            string locationSetting = Utility.GetPreferredLocation(Activity);

            string sortOrder = WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " ASC";
            Android.Net.Uri weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithStartDate(
                                                        locationSetting, DateTime.Now.Date.Ticks);

            return new CursorLoader(Activity, weatherForLocationUri, FORECAST_COLUMNS, null, null, sortOrder);
        }

        public void OnLoadFinished (Loader loader, Java.Lang.Object data)
        {
            var cursor = (ICursor)data;
            forecastAdapter.SwapCursor(cursor);
            if (position != RecyclerView.NoPosition)
                recyclerView.SmoothScrollToPosition(position);

            UpdateEmptyView();
        }
        public void OnLoaderReset (Loader loader)
        {
            forecastAdapter.SwapCursor(null);
        }

        public void setUseTodayLayout (bool useTodayLayout)
        {
            _useTodayLayout = useTodayLayout;
            forecastAdapter?.SetUseTodayLayout(_useTodayLayout);
        }

        private void UpdateEmptyView ()
        {
            if (forecastAdapter.ItemCount == 0)
            {

                var tv = (TextView)View.FindViewById(Resource.Id.recyclerview_forecast_empty);

                if (tv != null)
                {
                    var locStatus = Utility.GetLocationStatus(Activity);

                    switch (locStatus)
                    {
                        case (int)Helpers.LocationStatus.LocationStatusServerDown:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_down);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusServerInvalid:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_error);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusUnkown:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_unknown);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusInvalid:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_invalid_location);
                            break;
                        default:
                            if (!Utility.CheckNetworkStatus(Activity))
                            {
                                tv.Text = "Weather information not available. No network connection.";
                            }
                            break;
                    }
                }

            }
        }

        public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(GetString(Resource.String.pref_location_status_key)))
            {
                UpdateEmptyView();
            }
        }

        public void OnClick (object sender, long date)
        {
            var vh = (ForecastAdapter.ForecastAdapterViewHolder) sender;
            string locationSetting = Utility.GetPreferredLocation(Activity);
            ((Callback)Activity)
                    .OnItemSelected(WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithDate(
                                    locationSetting, date)
                    );
            position = vh.AdapterPosition;
        }
    }



}

