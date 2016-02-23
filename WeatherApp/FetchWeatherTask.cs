using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.App;
using Org.Json;
using Android.Content.Res;

namespace WeatherApp
{
	public class FetchWeatherTask
	{
		public FetchWeatherTask ()
		{
		}

		Context context = Application.Context;

		public async Task<String[]> FetchWeatherTaskFromZip (string zipCode)
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
			long roundedHigh = (long)convertTempUnits (Math.Round (high));
			long roundedLow = (long)convertTempUnits (Math.Round (low));

			String highLowStr = roundedHigh + "/" + roundedLow;
			return highLowStr;
		}

		private double convertTempUnits (double temp)
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (context);
			var tempUnit = prefs.GetString (context.GetString (Resource.String.pref_temp_key), context.GetString (Resource.String.pref_temp_default));
			if (tempUnit == "imperial") {
				return temp * 1.8 + 32;
			}
			return temp;
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

