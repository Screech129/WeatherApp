
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

namespace WeatherApp
{
	[Activity (Label = "SettingsActivity")]			
	public class SettingsActivity : Activity
	{
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_detail);
			// For all preferences, attach an OnPreferenceChangeListener so the UI summary can be
			// updated when the preference changes.
			FragmentTransaction fragTx = this.FragmentManager.BeginTransaction ();

			fragTx.Replace (Resource.Id.container, new SettingsFragment ())
				.Commit ();

		}
			

	}
}

