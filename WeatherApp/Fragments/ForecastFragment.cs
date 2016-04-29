﻿
using System;
using System.Collections.Generic;
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

namespace WeatherApp
{
    public class ForecastFragment : Fragment, LoaderManager.ILoaderCallbacks
    {

        ForecastAdapter forecastAdapter;
        private const String LOG_TAG = "ForecastAdapter";
        private const int URL_LOADER = 0;
        private int position;
        private ListView listView;
        private string[] FORECAST_COLUMNS = {
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

        private bool _useTodayLayout;

        public interface Callback
        {
            void OnItemSelected (Android.Net.Uri dateUri);
        }

        public ForecastFragment ()
        {

        }

        public void setUseTodayLayout (bool useTodayLayout)
        {
            _useTodayLayout = useTodayLayout;
            if (forecastAdapter != null)
            {
                forecastAdapter.setUseTodayLayout(_useTodayLayout);
            }
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
            updateWeather();
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            forecastAdapter = new ForecastAdapter(Activity, null, 0);
            forecastAdapter.setUseTodayLayout(_useTodayLayout);
            LoaderManager.InitLoader(URL_LOADER, null, this);
            var view = inflater.Inflate(Resource.Layout.fragment_main, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listview_forecast);
            listView.Adapter = forecastAdapter;
            listView.ItemClick += (sender, e) =>
            {
                position = e.Position;
                ICursor cursor = (ICursor)listView.Adapter.GetItem(e.Position);
                if (cursor != null)
                {
                    string locationSetting = Utility.getPreferredLocation(Activity);
                    ((Callback)Activity).OnItemSelected(WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate(locationSetting, cursor.GetLong(COL_WEATHER_DATE)));

                }

            };

            if (savedInstanceState != null && savedInstanceState.ContainsKey("position"))
            {
                position = savedInstanceState.GetInt("position");
            }

            return view;
        }

        public override void OnSaveInstanceState (Bundle outState)
        {
            if (position != ListView.InvalidPosition)
            {
                outState.PutInt("position", position);
            }
            base.OnSaveInstanceState(outState);
        }

        public Loader OnCreateLoader (int id, Bundle args)
        {
            string locationSetting = Utility.getPreferredLocation(Activity);

            string sortOrder = WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " ASC";
            Android.Net.Uri weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithStartDate(
                                                        locationSetting, DateTime.Now.Ticks);

            return new CursorLoader(Activity, weatherForLocationUri, FORECAST_COLUMNS, null, null, sortOrder);
        }

        public void OnLoaderReset (Loader loader)
        {
            forecastAdapter.SwapCursor(null);
        }

        public void OnLoadFinished (Loader loader, Java.Lang.Object data)
        {
            forecastAdapter.SwapCursor((ICursor)data);
            listView.SmoothScrollToPosition(position);
        }

        public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.forecastfragment, menu);
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_refresh)
            {
                updateWeather();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void updateWeather ()
        {
            
            SunshineSyncAdapter.SyncImmediately(Activity);
        }

        public void OnLocationChanged ()
        {
            updateWeather();
            LoaderManager.RestartLoader(URL_LOADER, null, this);
        }


    }



}

