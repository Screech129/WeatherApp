using System;

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
using WeatherApp.Sync;

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

            SunshineSyncAdapter.InitializeSyncAdapter(this);
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
			

			return base.OnOptionsItemSelected (item);
                 
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


