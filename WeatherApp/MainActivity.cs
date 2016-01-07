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

namespace WeatherApp
{
	[Activity (Label = "WeatherApp", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main,menu);
			return base.OnCreateOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_settings) {
				StartActivity (new Intent(this,typeof(SettingsActivity)));
				return true;
			}

			return base.OnOptionsItemSelected (item);

		}



	}
}


