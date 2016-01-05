
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

				Context context = this.Activity;
				string text = ForecastAdapter.GetItem ((int)e.Id).ToString ();


				Toast toast = Toast.MakeText (context, text, ToastLength.Short);
				toast.Show ();
			};

			return view;
		}
				


		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			menu.Clear ();
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
				var forecastResult = await FetchWeatherTask ("70809");
				ForecastAdapter.AddAll (forecastResult.ToList ());
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public async Task<String[]> FetchWeatherTask (string zipCode)
		{

			// These two need to be declared outside the try/catch
			// so that they can be closed in the finally block.
			// Will contain the raw JSON response as a string.
			StreamReader reader = null;
			try {

				var httpClient = new HttpClient ();
				// Construct the URL for the OpenWeatherMap query
				// Possible parameters are available at OWM's forecast API page, at
				// http://openweathermap.org/API#forecast
				Task<string> getJSON = httpClient.GetStringAsync ("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + zipCode + ",us&mode=json&units=metric&cnt=7$APPID=003b1510993370c1cb38d040291c4f18");
				string JSON = await getJSON;

				// Read the input stream into a String

				var stringBuilder = new StringBuilder ();
				stringBuilder.Append (JSON);

				if (stringBuilder.Length == 0) {
					// Stream was empty.  No point in parsing.
					return null;
				}
				return getWeatherDataFromJson (stringBuilder.ToString (), 7);
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

		private String getReadableDateString (DateTime time)
		{
			// Because the API returns a unix timestamp (measured in seconds),
			// it must be converted to milliseconds in order to be converted to valid date.
			//var formattedDate = new DateTime (time);
			return time.ToString ("ddd MMM dd");
			;
		}

		/**
         * Prepare the weather high/lows for presentation.
         */
		private String formatHighLows (double high, double low)
		{
			// For presentation, assume the user doesn't care about tenths of a degree.
			long roundedHigh = (long)Math.Round (high);
			long roundedLow = (long)Math.Round (low);

			String highLowStr = roundedHigh + "/" + roundedLow;
			return highLowStr;
		}

		/**
         * Take the String representing the complete forecast in JSON Format and
         * pull out the data we need to construct the Strings needed for the wireframes.
         *
         * Fortunately parsing is easy:  constructor takes the JSON string and converts it
         * into an Object hierarchy for us.
         */
		private String[] getWeatherDataFromJson (String forecastJsonStr, int numDays)
		{

			// These are the names of the JSON objects that need to be extracted.
			const String OWM_LIST = "list";
			const String OWM_WEATHER = "weather";
			const String OWM_TEMPERATURE = "temp";
			const String OWM_MAX = "max";
			const String OWM_MIN = "min";
			const String OWM_DESCRIPTION = "main";

			JSONObject forecastJson = new JSONObject (forecastJsonStr);
			JSONArray weatherArray = forecastJson.GetJSONArray (OWM_LIST);

			// OWM returns daily forecasts based upon the local time of the city that is being
			// asked for, which means that we need to know the GMT offset to translate this data
			// properly.

			// Since this data is also sent in-order and the first day is always the
			// current day, we're going to take advantage of that to get a nice
			// normalized UTC date for all of our weather.

			DateTime dayTime = DateTime.UtcNow;


			String[] resultStrs = new String[numDays];
			for (int i = 0; i < weatherArray.Length (); i++) {
				// For now, using the format "Day, description, hi/low"
				String day;
				String description;
				String highAndLow;

				// Get the JSON object representing the day
				JSONObject dayForecast = weatherArray.GetJSONObject (i);

				// The date/time is returned as a long.  We need to convert that
				// into something human-readable, since most people won't read "1400356800" as
				// "this saturday".
				DateTime dateTime;
				// Cheating to convert this to UTC time, which is what we want anyhow
				dateTime = dayTime.AddDays (i);
				day = getReadableDateString (dateTime);

				// description is in a child array called "weather", which is 1 element long.
				JSONObject weatherObject = dayForecast.GetJSONArray (OWM_WEATHER).GetJSONObject (0);
				description = weatherObject.GetString (OWM_DESCRIPTION);

				// Temperatures are in a child object called "temp".  Try not to name variables
				// "temp" when working with temperature.  It confuses everybody.
				JSONObject temperatureObject = dayForecast.GetJSONObject (OWM_TEMPERATURE);
				double high = temperatureObject.GetDouble (OWM_MAX);
				double low = temperatureObject.GetDouble (OWM_MIN);

				highAndLow = formatHighLows (high, low);
				resultStrs [i] = day + " - " + description + " - " + highAndLow;
			}
			return resultStrs;

		}

	}
}

