
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Preferences;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Org.Json;

namespace WeatherApp
{
	public class ForecastFragment : Fragment
	{
		ArrayAdapter ForecastAdapter;
		private const String LOG_TAG = "ForecastAdapter";

		public ForecastFragment ()
		{
			
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			if (savedInstanceState != null) {
				return;
			}
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			ForecastAdapter = new ArrayAdapter<string> (Activity, Resource.Layout.list_item_forecast, 0);
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			
			var view = inflater.Inflate (Resource.Layout.fragment_main, container, false);
			var listView = view.FindViewById<ListView> (Resource.Id.listview_forecast);
			listView.Adapter = ForecastAdapter;

			listView.ItemClick += (sender, e) => {
				var foreCast = ForecastAdapter.GetItem ((int)e.Id);
				var detailsIntent = new Intent (this.Activity, typeof(DetailActivity));
				detailsIntent.PutExtra (Intent.ExtraText, foreCast.ToString ());
				StartActivity (detailsIntent);
			};

			return view;
		}



		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.forecastfragment, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			var ignoreTask = OnOptionsItemSelectedAsync (item);

			return true;
		}

		public async Task<bool> OnOptionsItemSelectedAsync (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_refresh) {
				await updateWeather ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public async Task updateWeather ()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (Activity);
			var zipCode = prefs.GetString (Resources.GetString (Resource.String.pref_location_key), Resources.GetString (Resource.String.pref_location_default));
			var weatherTask = new FetchWeatherTask ();
			var forecastResult = await weatherTask.FetchWeatherTaskFromZip (zipCode);
			ForecastAdapter.Clear ();
			ForecastAdapter.AddAll (forecastResult.ToList ());
		}

		public override void OnStart ()
		{
			base.OnStart ();
			updateWeather ();
		}



	}
}

