
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

namespace WeatherApp
{
	public class ForecastFragment : Fragment,LoaderManager.ILoaderCallbacks
	{
		ForecastAdapter forecastAdapter;
		private const String LOG_TAG = "ForecastAdapter";
		private const int URL_LOADER = 0;

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

		public ForecastFragment ()
		{
			
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			if (savedInstanceState != null) {
				return;
			}
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			forecastAdapter = new ForecastAdapter (Activity, null, 0);
			LoaderManager.InitLoader (URL_LOADER, null, this);
			var view = inflater.Inflate (Resource.Layout.fragment_main, container, false);
			var listView = view.FindViewById<ListView> (Resource.Id.listview_forecast);
			listView.Adapter = forecastAdapter;

			listView.ItemClick += (sender, e) => {
				
				ICursor cursor = (ICursor)listView.Adapter.GetItem (e.Position);
				if (cursor != null) {
					string locationSetting = Utility.getPreferredLocation (Activity);
					Intent intent = new Intent (Activity, typeof(DetailActivity))
						.SetData (WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (locationSetting, cursor.GetLong (COL_WEATHER_DATE)));
					StartActivity (intent);
				}

			};


			return view;
		}

		public Loader OnCreateLoader (int id, Bundle args)
		{
			string locationSetting = Utility.getPreferredLocation (Activity);

			String sortOrder = WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " ASC";
			Android.Net.Uri weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithStartDate (
				                                        locationSetting, DateTime.Now.Ticks);
			
			return new CursorLoader (Activity, weatherForLocationUri, FORECAST_COLUMNS, null, null, sortOrder);
		}

		public void OnLoaderReset (Loader loader)
		{
			forecastAdapter.SwapCursor (null);
		}

		public void OnLoadFinished (Loader loader, Java.Lang.Object data)
		{
			forecastAdapter.SwapCursor ((ICursor)data);
		}




		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.forecastfragment, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			var ignoreTask = OnOptionsItemSelectedAsync (item);

			return true;
		}

		public async Task<bool> OnOptionsItemSelectedAsync (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_refresh) {
				await updateWeather ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public async Task updateWeather ()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (Activity);
			var zipCode = prefs.GetString (Resources.GetString (Resource.String.pref_location_key), Resources.GetString (Resource.String.pref_location_default));
			var weatherTask = new FetchWeatherTask (this.Activity);
			await weatherTask.FetchWeatherTaskFromZip (zipCode);

		}

		public void OnLocationChanged ()
		{
			updateWeather ();
			LoaderManager.RestartLoader (URL_LOADER, null, this);
		}
			
	}
}

