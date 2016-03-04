
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
using Android.Util;
using Android.Database;

namespace WeatherApp
{
	[Activity (Label = "DetailActivity")]			
	public class DetailActivity : Activity
	{
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (WindowFeatures.ActionBar);
			SetContentView (Resource.Layout.activity_detail);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		
			if (savedInstanceState == null) {
				FragmentTransaction fragTx = this.FragmentManager.BeginTransaction ();

				fragTx.Add (Resource.Id.container, new PlaceholderFragment ())
					.Commit ();
			   


			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.detail, menu);
			return base.OnCreateOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Android.Resource.Id.Home) {
				Finish ();
			}
			if (id == Resource.Id.action_settings) {
				var settingsIntent = new Intent (this, typeof(SettingsActivity));
				StartActivity (settingsIntent);
				return true;
			}
			if (id == Resource.Id.action_share) {
				new PlaceholderFragment ().shareWeather (item);
				return true;
			}
			return base.OnOptionsItemSelected (item);

		}

		public  class PlaceholderFragment:Fragment,LoaderManager.ILoaderCallbacks
		{
			public PlaceholderFragment ()
			{
				SetHasOptionsMenu (true);
			}

			Android.Net.Uri forecast;
			string forecastString = "";
			private const int URL_LOADER = 0;

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
				View rootView = inflater.Inflate (Resource.Layout.fragment_detail, container, false);
				LoaderManager.InitLoader (URL_LOADER, null, this);

				return rootView;
			}

			public void shareWeather (IMenuItem item)
			{
				var shareText = forecast + " #SunshineApp";
				var shareIntent = new Intent (Intent.ActionSend)
					.AddFlags (ActivityFlags.ClearWhenTaskReset)
					.SetType ("text/plain")
					.PutExtra (Intent.ExtraText, shareText);
				var shareActionProvider = (ShareActionProvider)item.ActionProvider;
				if (shareActionProvider != null) {

					shareActionProvider.SetShareIntent (shareIntent);
				} else {
					Log.Debug ("DetailFragment", "Share Action Provider May Be Null");
				}

			}

			public Loader OnCreateLoader (int id, Bundle args)
			{
				Intent intent = Activity.Intent;

				if (intent == null) {
					return null;
				}

				return new CursorLoader (Activity, intent.Data, null, null, null, null);
			}


			public void OnLoaderReset (Loader loader)
			{
				forecastString = "";
			}

			public void OnLoadFinished (Loader loader, Java.Lang.Object data)
			{
				var cursor = (ICursor)data;
				if (cursor.MoveToNext ())
					forecastString = convertCursorRowToUXFormat (cursor);
				var tv = (TextView)View.FindViewById<TextView> (Resource.Id.detail_text);
				tv.Text = forecastString;
			}

			private String formatHighLows (double high, double low)
			{
				bool isMetric = Utility.isMetric (Activity);
				String highLowStr = Utility.formatTemperature (high, isMetric) + "/" + Utility.formatTemperature (low, isMetric);
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
					                    cursor.GetDouble (ForecastFragment.COL_WEATHER_MAX_TEMP),
					                    cursor.GetDouble (ForecastFragment.COL_WEATHER_MIN_TEMP));

				return Utility.formatDate (cursor.GetLong (ForecastFragment.COL_WEATHER_DATE)) +
				" - " + cursor.GetString (ForecastFragment.COL_WEATHER_DESC) +
				" - " + highAndLow;
			}
		}
	}
}

