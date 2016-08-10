
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
	public class DetailActivity : AppCompatActivity
	{
		

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_detail);
           
            if (savedInstanceState == null) {
				Bundle arguments = new Bundle ();
				arguments.PutParcelable (DetailFragment.DETAIL_URI, Intent.Data);

				DetailFragment fragment = new DetailFragment ();
				fragment.Arguments = arguments;

				SupportFragmentManager.BeginTransaction().Add(Resource.Id.weather_detail_container, fragment)
					.Commit ();
			}
		}

		
	}
}

