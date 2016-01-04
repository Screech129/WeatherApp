
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace WeatherApp
{
	public class ForecaseFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
						FetchWeatherTask ();

						return base.OnCreateView (inflater, container, savedInstanceState);
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.forecastfragment,menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var ignoreTask = OnOptionsItemSelectedAsync(item);

			return true;
		}

		public async Task<bool> OnOptionsItemSelectedAsync (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_refresh) {
				await FetchWeatherTask ("70809");
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public async Task<String> FetchWeatherTask(string zipCode){

			// These two need to be declared outside the try/catch
			// so that they can be closed in the finally block.
			// Will contain the raw JSON response as a string.
			StreamReader reader = null;
			const String LOG_TAG = "FetchWeatherTask";
			try {
				// Construct the URL for the OpenWeatherMap query
				// Possible parameters are available at OWM's forecast API page, at
				// http://openweathermap.org/API#forecast
				var url = new Uri ("http://api.openweathermap.org/data/2.5/forecast/daily?q="+zipCode+"&mode=json&units=metric&cnt=7$APPID=003b1510993370c1cb38d040291c4f18");

				// Create the request to OpenWeatherMap, and open the connection
				var request = (HttpWebRequest)WebRequest.Create (url);
				var response = (HttpWebResponse)request.GetResponse ();

				// Read the input stream into a String

				reader = new StreamReader (response.GetResponseStream ());
				var stringBuilder = new StringBuilder ();
				string line;
				while ((line = reader.ReadLine ()) != null) {
					// Since it's JSON, adding a newline isn't necessary (it won't affect parsing)
					// But it does make debugging a *lot* easier if you print out the completed
					// buffer for debugging.
					stringBuilder.Append (line + "\n");
				}

				if (stringBuilder.Length == 0) {
					// Stream was empty.  No point in parsing.
					return null;
				}
				Log.Verbose (LOG_TAG,"Forecast JSON String: " + stringBuilder.ToString ());
				return stringBuilder.ToString ();
			} catch (IOException e) {
				Log.WriteLine (LogPriority.Error, "PlaceholderFragment", "Error ", e);
				// If the code didn't successfully get the weather data, there's no point in attempting
				// to parse it.
				return null;
			} finally {
				if (reader != null) {
					try {
						reader.Close ();
					} catch (IOException e) {
						Log.WriteLine (LogPriority.Error, "PlaceholderFragment", "Error closing stream", e);
					}
				}

		}
	}
}
}

