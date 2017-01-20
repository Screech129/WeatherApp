
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Preferences;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace WeatherApp
{
	[Activity (Label = "SettingsActivity", Theme = "@style/SettingsTheme")]
	public class SettingsActivity : Activity
	{
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_detail);

			//ActionBar.SetDisplayHomeAsUpEnabled (true);
			//ActionBar.SetDisplayShowHomeEnabled (true);
			//ActionBar.Title = "Settings";
			// For all preferences, attach an OnPreferenceChangeListener so the UI summary can be
			// updated when the preference changes.
			var fragTx = this.FragmentManager.BeginTransaction ();

			fragTx.Replace (Resource.Id.weather_detail_container, new SettingsFragment ())
				.Commit ();

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			return base.OnCreateOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			var id = item.ItemId;
			if (id == Android.Resource.Id.Home) {
				Finish ();
			}
			return base.OnOptionsItemSelected (item);

		}

			

	}
}

