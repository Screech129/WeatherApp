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
			FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction ();
			ForecaseFragment forecast = new ForecaseFragment ();
			fragmentTx.Add (Resource.Id.listview_forecast, forecast);
			fragmentTx.Commit ();
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.fragment_main);
			var weekForecast = new List<string>(){
				"Today-Sunny-88/63",
				"Tomorrow-Foggy-70/46",
				"Weds-Cloudy-72/63"
			};

			var listView = FindViewById<ListView> (Resource.Id.listview_forecast);
			var mForecastAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item_forecast, Resource.Id.list_item_forecast_textview, weekForecast);
			listView.Adapter = mForecastAdapter;

		}
			



	}
}


