using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Database;
using Com.Bumptech.Glide;
using Android.Support.V4;
using Android.Support.V4.View;

namespace WeatherApp
{
	public class DetailFragment: Fragment,LoaderManager.ILoaderCallbacks
	{
		public DetailFragment ()
		{
			SetHasOptionsMenu (true);
		}

		Android.Net.Uri forecast;
		string forecastString = "";
		private const int URL_LOADER = 0;
		public const string DETAIL_URI = "URI";
		private Android.Net.Uri globalUri;
		ImageView iconView;
		TextView dateView;
		TextView friendlyDateView;
		TextView highTempView;
		TextView lowTempView;
		TextView humidityView;
		TextView windView;
		TextView pressureView;
		TextView descriptionView;


		private string[] DETAIL_COLUMNS = {
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
			WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id,
			WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY,
			WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED,
			WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE,
			WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES,

		};

		// These indices are tied to FORECAST_COLUMNS.  If FORECAST_COLUMNS changes, these
		// must change.
		public const int COL_WEATHER_ID = 0;
		public const int COL_WEATHER_DATE = 1;
		public const int COL_WEATHER_DESC = 2;
		public const int COL_WEATHER_MAX_TEMP = 3;
		public const int COL_WEATHER_MIN_TEMP = 4;
		public const int COL_WEATHER_CONDITION_ID = 5;
		public const int COL_WEATHER_HUMIDITY = 6;
		public const int COL_WEATHER_WIND_SPEED = 7;
		public const int COL_WEATHER_PRESSURE = 8;
		public const int COL_WEATHER_DEGREES = 9;


		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.detail_fragment, menu);
			IMenuItem menuItem = menu.FindItem (Resource.Id.action_share);
			shareWeather (menuItem);
			base.OnCreateOptionsMenu (menu, inflater);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_share) {
				shareWeather (item);
				return true;
			}
			return base.OnOptionsItemSelected (item);

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Bundle arguments = Arguments;
			if (arguments != null) {
				globalUri = (Android.Net.Uri)arguments.GetParcelable (DetailFragment.DETAIL_URI);
			}

			View rootView = inflater.Inflate (Resource.Layout.fragment_detail, container, false);
			iconView = rootView.FindViewById<ImageView> (Resource.Id.detail_icon);
			dateView = rootView.FindViewById<TextView> (Resource.Id.detail_date_textview);
			friendlyDateView = rootView.FindViewById<TextView> (Resource.Id.detail_day_textview);
			descriptionView = rootView.FindViewById<TextView> (Resource.Id.detail_forecast_textview);
			highTempView = rootView.FindViewById<TextView> (Resource.Id.detail_high_textview);
			lowTempView = rootView.FindViewById<TextView> (Resource.Id.detail_low_textview);
			humidityView = rootView.FindViewById<TextView> (Resource.Id.detail_humidity_textview);
			windView = rootView.FindViewById<TextView> (Resource.Id.detail_wind_textview);
			pressureView = rootView.FindViewById<TextView> (Resource.Id.detail_pressure_textview);



			return rootView;
		}

		public void shareWeather (IMenuItem item)
		{
			var shareText = forecastString + " #SunshineApp";
			var shareIntent = new Intent (Intent.ActionSend)
					.AddFlags (ActivityFlags.ClearWhenTaskReset)
					.SetType ("text/plain")
					.PutExtra (Intent.ExtraText, shareText);
			var shareActionProvider = (Android.Support.V7.Widget.ShareActionProvider)MenuItemCompat.GetActionProvider(item);
			if (shareActionProvider != null) {

				shareActionProvider.SetShareIntent (shareIntent);
			} else {
				Log.Debug ("DetailFragment", "Share Action Provider May Be Null");
			}

		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			LoaderManager.InitLoader (URL_LOADER, null, this);
			base.OnActivityCreated (savedInstanceState);
		}

		public Loader OnCreateLoader (int id, Bundle args)
		{

			if (globalUri != null) {
				
				return new CursorLoader (Activity, globalUri, DETAIL_COLUMNS, null, null, null);
			}
			return null;
		}


		public void OnLoaderReset (Loader loader)
		{

		}

		public void OnLocationChanged (string newLocation)
		{
			Android.Net.Uri uri = globalUri;
			if (uri != null) {
				long date = WeatherContractOpen.WeatherEntryOpen.getDateFromUri (uri);
				Android.Net.Uri updatedUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (newLocation, date);
				globalUri = updatedUri;
				LoaderManager.RestartLoader (URL_LOADER, null, this);
			}
		}

		public void OnLoadFinished (Loader loader, Java.Lang.Object data)
		{
			var cursor = (ICursor)data;
			if (cursor != null && cursor.MoveToNext ()) {
				
			

				var date = cursor.GetLong (COL_WEATHER_DATE);
				friendlyDateView.Text = Utility.getDayName (Activity, date);
				dateView.Text = Utility.getFormattedMonthDay (Activity, date);

				var isMetric = Utility.isMetric (Activity);

				var high = cursor.GetDouble (COL_WEATHER_MAX_TEMP);
				highTempView.Text = Utility.formatTemperature (Activity, high, isMetric);

				var low = cursor.GetDouble (COL_WEATHER_MIN_TEMP);
				lowTempView.Text = Utility.formatTemperature (Activity, low, isMetric);

				var weatherId = cursor.GetInt (COL_WEATHER_CONDITION_ID);
			    Glide.With(this)
                    .Load(Utility.GetArtUrlForWeatherCondition(Activity, weatherId))
                    .Error(Utility.GetArtResourceForWeatherCondition(weatherId))
			        .Into(iconView);

				var forecastText = cursor.GetString (COL_WEATHER_DESC);
				descriptionView.Text = forecastText;

				var humidity = cursor.GetLong (COL_WEATHER_HUMIDITY);
				humidityView.Text = String.Format (Activity.GetString (Resource.String.format_humidity), humidity);

				var wind = cursor.GetLong (COL_WEATHER_WIND_SPEED);
				windView.Text = Utility.getFormattedWind (Activity, wind, cursor.GetLong (COL_WEATHER_DEGREES));

				var pressure = cursor.GetLong (COL_WEATHER_PRESSURE);
				pressureView.Text = String.Format (Activity.GetString (Resource.String.format_pressure), pressure);

				forecastString = convertCursorRowToUXFormat (cursor);

			}
		}

		private String formatHighLows (double high, double low)
		{
			bool isMetric = Utility.isMetric (Activity);
			String highLowStr = Utility.formatTemperature (Activity, high, isMetric) + "/" + Utility.formatTemperature (Activity, low, isMetric);
			return highLowStr;
		}

		/*
        This is ported from FetchWeatherTask --- but now we go straight from the cursor to the
        string.
     */
		private String convertCursorRowToUXFormat (ICursor cursor)
		{
			// get row indices for our cursor

			String highAndLow = formatHighLows (
				                    cursor.GetDouble (COL_WEATHER_MAX_TEMP),
				                    cursor.GetDouble (COL_WEATHER_MIN_TEMP));

			return Utility.formatDate (cursor.GetLong (COL_WEATHER_DATE)) +
			" - " + cursor.GetString (COL_WEATHER_DESC) +
			" - " + highAndLow;
		}


	}


}

