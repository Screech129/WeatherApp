﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Database;
using Android.Support.V7.App;

namespace WeatherApp
{
	[Activity (Label = "DetailActivity")]			
	public class DetailActivity : ActionBarActivity
	{
		

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (WindowFeatures.ActionBar);
			SetContentView (Resource.Layout.activity_detail);
            var toolbar = (Toolbar)FindViewById(Resource.Id.toolbarDetail);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (savedInstanceState == null) {
				Bundle arguments = new Bundle ();
				arguments.PutParcelable (DetailFragment.DETAIL_URI, Intent.Data);

				DetailFragment fragment = new DetailFragment ();
				fragment.Arguments = arguments;

				FragmentTransaction fragTx = this.FragmentManager.BeginTransaction ();

				fragTx.Add (Resource.Id.weather_detail_container, fragment)
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
				new DetailFragment ().shareWeather (item);
				return true;
			}
			return base.OnOptionsItemSelected (item);

		}

	}
}

