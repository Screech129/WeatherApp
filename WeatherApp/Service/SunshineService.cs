using System;
using Android.App;
using Org.Json;
using System.Collections;
using Android.Content;
using Android.Util;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp
{
	public class SunshineService: IntentService
	{
		Context _context;

		public SunshineService ()
		{
			_context = this;
		}


		protected override void OnHandleIntent (Android.Content.Intent intent)
		{
			StreamReader reader = null;
			string zipCode = intent.GetStringExtra ("lqe");
			try {

				var httpClient = new HttpClient ();
				// Construct the URL for the OpenWeatherMap query
				// Possible parameters are available at OWM's forecast API page, at
				// http://openweathermap.org/API#forecast
				Task<string> getJSON = httpClient.GetStringAsync ("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + zipCode + ",us&mode=json&units=metric&cnt=7&APPID=83fde89b086ca4abec16cb2a8c245bb8");
				string JSON = getJSON.Result;

				getWeatherDataFromJson (JSON, zipCode);
			} catch (IOException e) {
				Log.WriteLine (LogPriority.Error, "PlaceholderFragment", "Error ", e);
				// If the code didn't successfully get the weather data, there's no point in attempting
				// to parse it.
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

		private void getWeatherDataFromJson (String forecastJsonStr, String locationSetting)
		{

			// These are the names of the JSON objects that need to be extracted.
			// Location information
			const string OWM_CITY = "city";
			const string OWM_CITY_NAME = "name";
			const string OWM_COORD = "coord";

			const string OWM_LATITUDE = "lat";
			const string OWM_LONGITUDE = "lon";

			const string OWM_LIST = "list";

			const string OWM_PRESSURE = "pressure";
			const string OWM_HUMIDITY = "humidity";
			const string OWM_WINDSPEED = "speed";
			const string OWM_WIND_DIRECTION = "deg";

			const string OWM_TEMPERATURE = "temp";
			const string OWM_MAX = "max";
			const string OWM_MIN = "min";

			const string OWM_WEATHER = "weather";
			const string OWM_DESCRIPTION = "main";
			const string OWM_WEATHER_id = "id";

			try {
				JSONObject forecastJson = new JSONObject (forecastJsonStr);
				JSONArray weatherArray = forecastJson.GetJSONArray (OWM_LIST);

				JSONObject cityJson = forecastJson.GetJSONObject (OWM_CITY);
				String cityName = cityJson.GetString (OWM_CITY_NAME);

				JSONObject cityCoord = cityJson.GetJSONObject (OWM_COORD);
				double cityLatitude = cityCoord.GetDouble (OWM_LATITUDE);
				double cityLongitude = cityCoord.GetDouble (OWM_LONGITUDE);

				long locationId = AddLocation (locationSetting, cityName, cityLatitude, cityLongitude);

				ArrayList jsonResultValues = new ArrayList ();

				DateTime dayTime = DateTime.UtcNow;


				for (int i = 0; i < weatherArray.Length (); i++) {
					// These are the values that will be collected.
					long dateTime;
					double pressure;
					int humidity;
					double windSpeed;
					double windDirection;

					double high;
					double low;

					string description;
					int weatherId;

					// Get the JSON object representing the day
					JSONObject dayForecast = weatherArray.GetJSONObject (i);

					// The date/time is returned as a long.  We need to convert that
					// into something human-readable, since most people won't read "1400356800" as
					// "this saturday".
					// Cheating to convert this to UTC time, which is what we want anyhow
					dateTime = dayTime.AddDays (i).Ticks;

					pressure = dayForecast.GetDouble (OWM_PRESSURE);
					humidity = dayForecast.GetInt (OWM_HUMIDITY);
					windSpeed = dayForecast.GetDouble (OWM_WINDSPEED);
					windDirection = dayForecast.GetDouble (OWM_WIND_DIRECTION);

					// Description is in a child array called "weather", which is 1 element long.
					// That element also contains a weather code.
					JSONObject weatherObject =
						dayForecast.GetJSONArray (OWM_WEATHER).GetJSONObject (0);
					description = weatherObject.GetString (OWM_DESCRIPTION);
					weatherId = weatherObject.GetInt (OWM_WEATHER_id);

					// Temperatures are in a child object called "temp".  Try not to name variables
					// "temp" when working with temperature.  It confuses everybody.
					JSONObject temperatureObject = dayForecast.GetJSONObject (OWM_TEMPERATURE);
					high = temperatureObject.GetDouble (OWM_MAX);
					low = temperatureObject.GetDouble (OWM_MIN);

					ContentValues weatherValues = new ContentValues ();

					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY, locationId);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE, dateTime);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY, humidity);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE, pressure);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED, windSpeed);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES, windDirection);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP, high);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP, low);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC, description);
					weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id, weatherId);

					jsonResultValues.Add (weatherValues);
				}
				BulkInsertWeather (jsonResultValues, locationSetting);

			} catch (Exception ex) {
				Log.Error ("Featch Weather Task", ex.Message);
			}


		}

		public long AddLocation (string locationSetting, string cityName, double lat, double lon)
		{
			var returnedId = "0";

			var location = _context.ContentResolver.Query (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, null, WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ?", new string[]{ locationSetting }, null);
			if (!location.MoveToFirst ()) {
				var locationValues = createLocationValues (locationSetting, cityName, lat, lon);
				var insertUri = _context.ContentResolver.Insert (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, locationValues);
				returnedId = insertUri.LastPathSegment;
			} else {
				var locationIdColumn = location.GetColumnIndex (WeatherContractOpen.LocationEntryOpen._ID);
				returnedId = location.GetInt (locationIdColumn).ToString ();
			}

			return long.Parse (returnedId);
		}


		public void BulkInsertWeather (ArrayList weatherValues, string locationSetting)
		{
			_context.ContentResolver.Delete (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, null, null);
			var insertedCount = 0;
			if (weatherValues.Count > 0) {
				var cvArray = new ContentValues[weatherValues.Count];

				cvArray = (ContentValues[])weatherValues.ToArray (typeof(ContentValues));
				insertedCount = _context.ContentResolver.BulkInsert (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, cvArray);
			}

			Log.Debug ("Fetch Weather Task", "FetchweatherTask Complete " + insertedCount + " records Inserted");
		}

		public static ContentValues createLocationValues (string locationSetting, string cityName, double lat, double lon)
		{

			// Create a new map of values, where column names are the keys
			ContentValues locValues = new ContentValues ();
			locValues.Put (WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING, locationSetting);
			locValues.Put (WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME, cityName);
			locValues.Put (WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT, lat);
			locValues.Put (WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG, lon);

			return locValues;
		}

	}
}

