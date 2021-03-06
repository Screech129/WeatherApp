﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Util;
using System.Net;
using System.IO;
using System.Text;
using Android.Preferences;

namespace WeatherApp
{
	[Activity (Label = "WeatherApp", MainLauncher = true)]
	public class MainActivity : Activity,WeatherApp.ForecastFragment.Callback
	{
		string location = "";
		private const string DETAILFRAGMENT_TAG = "DFTAG";
		bool twoPane;

		protected override void OnCreate (Bundle bundle)
		{
			location = Utility.getPreferredLocation (this);
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Resource.Drawable.ic_logo);
			ActionBar.Title = "";

			if (FindViewById (Resource.Id.weather_detail_container) != null) {
				twoPane = true;
				if (bundle == null) {
					FragmentManager.BeginTransaction ()
						.Replace (Resource.Id.weather_detail_container,
						new DetailFragment (), DETAILFRAGMENT_TAG)
						.Commit ();

				} 
			} else {
				twoPane = false;
			}

			ForecastFragment forecastFragment = FragmentManager.FindFragmentById<ForecastFragment> (Resource.Id.fragment_forecast);
			forecastFragment.setUseTodayLayout (!twoPane);
		}

		public void OnItemSelected (Android.Net.Uri dateUri)
		{
			
			if (twoPane) {
				Bundle args = new Bundle ();
				args.PutParcelable (DetailFragment.DETAIL_URI, dateUri);
			
				DetailFragment fragment = new DetailFragment ();
				fragment.Arguments = args;
			
				FragmentManager.BeginTransaction ()
			             .Replace (Resource.Id.weather_detail_container, fragment, DETAILFRAGMENT_TAG)
			             .Commit ();
			} else {
				Intent intent = new Intent (this, typeof(DetailActivity))
			                 .SetData (dateUri);
				StartActivity (intent);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return base.OnCreateOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_settings) {
				StartActivity (new Intent (this, typeof(SettingsActivity)));
				return true;
			}
			if (id == Resource.Id.action_viewLocation) {
				openPreferredLocationInMap ();
				return true;
			}

			return base.OnOptionsItemSelected (item);
                 
		}

		private void openPreferredLocationInMap ()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (this);
			var zipCode = prefs.GetString (Resources.GetString (Resource.String.pref_location_key), Resources.GetString (Resource.String.pref_location_default));

			var geoLocation = Android.Net.Uri.Parse ("geo:0,0?")
    				.BuildUpon ()
    				.AppendQueryParameter ("q", zipCode)
    				.Build ();
			var mapIntent = new Intent (Intent.ActionView, geoLocation);
			if (mapIntent.ResolveActivity (this.PackageManager) != null) {
				StartActivity (mapIntent);
			} else {
				Toast.MakeText (this, "No Map App found", ToastLength.Long).Show ();
				Log.Debug ("Main Activity", "Couldn't call " + zipCode + ", No Maps App");
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

		protected override void OnResume ()
		{
			base.OnResume ();
			if (Utility.getPreferredLocation (this) != location) {
				ForecastFragment ff = FragmentManager.FindFragmentById <ForecastFragment> (Resource.Id.fragment_forecast);
				if (ff != null) {					
					ff.OnLocationChanged ();
				}

				DetailFragment df = FragmentManager.FindFragmentByTag<DetailFragment> (DETAILFRAGMENT_TAG);
				if (df != null) {
					df.OnLocationChanged (location);
				}
				location = Utility.getPreferredLocation (this);
			}

		}

	}
}


