using Android.Net;
using System;
using Android.Content;
using System.Globalization;

namespace WeatherApp
{
	public class WeatherContractOpen
	{
		public WeatherContractOpen ()
		{
		}


		// The "Content authority" is a name for the entire content provider, similar to the
		// relationship between a domain name and its website.  A convenient string to use for the
		// content authority is the package name for the app, which is guaranteed to be unique on the
		// device.
		public const string CONTENT_AUTHORITY = "WeatherApp";

		// Use CONTENT_AUTHORITY to create the base of all URI's which apps will use to contact
		// the content provider.
		public static Android.Net.Uri BASE_CONTENT_URI = Android.Net.Uri.Parse ("content://" + CONTENT_AUTHORITY);

		// Possible paths (appended to base content URI for possible URI's)
		// For instance, content://com.example.android.sunshine.app/weather/ is a valid path for
		// looking at weather data. content://com.example.android.sunshine.app/givemeroot/ will fail,
		// as the ContentProvider hasn't been given any information on what to do with "givemeroot".
		// At least, let's hope not.  Don't be that dev, reader.  Don't be that dev.
		public const string PATH_WEATHER = "weather";
		public const string PATH_LOCATION = "location";

		readonly static JulianCalendar cal = new JulianCalendar ();

		public static long normalizeDate (long startDate)
		{
			// normalize the start date to the beginning of the (UTC) day
			DateTime time = new DateTime ();
			time = time.AddTicks (startDate);
			return time.Ticks;
		}

		public   class LocationEntryOpen:BaseColumns
		{
			public static  Android.Net.Uri CONTENT_URI =
				BASE_CONTENT_URI.BuildUpon ().AppendPath (PATH_LOCATION).Build ();

			public const String CONTENT_TYPE =
				ContentResolver.CursorDirBaseType + "/" + CONTENT_AUTHORITY + "/" + PATH_LOCATION;
			public const String CONTENT_ITEM_TYPE =
				ContentResolver.CursorDirBaseType + "/" + CONTENT_AUTHORITY + "/" + PATH_LOCATION;

			public static Android.Net.Uri buildLocationUri (long id)
			{
				return ContentUris.WithAppendedId (CONTENT_URI, id);
			}

			public  const string TABLE_NAME = "location";
			public const string COLUMN_LOCATION_SETTING = "location_setting";
			public const  string COLUMN_CITY_NAME = "city_name";
			public const  string COLUMN_COORD_LAT = "coord_lat";
			public const  string COLUMN_COORD_LONG = "coord_long";
		}

		/* Inner class that defines the table contents of the weather table */
		public  class WeatherEntryOpen:BaseColumns
		{
			public static  Android.Net.Uri CONTENT_URI =
				BASE_CONTENT_URI.BuildUpon ().AppendPath (PATH_WEATHER).Build ();

			public static string CONTENT_TYPE =
				ContentResolver.CursorDirBaseType + "/" + CONTENT_AUTHORITY + "/" + PATH_WEATHER;
			public static  string CONTENT_ITEM_TYPE =
				ContentResolver.CursorItemBaseType + "/" + CONTENT_AUTHORITY + "/" + PATH_WEATHER;


			public static Android.Net.Uri buildWeatherUri (long id)
			{
				return ContentUris.WithAppendedId (CONTENT_URI, id);
			}

			/*
            Student: Fill in this buildWeatherLocation function
         */
			public static Android.Net.Uri buildWeatherLocation (String locationSetting)
			{
				return CONTENT_URI.BuildUpon ().AppendPath (locationSetting).Build ();
			}

			public static Android.Net.Uri buildWeatherLocationWithStartDate (
				String locationSetting, long startDate)
			{
				long normalizedDate = normalizeDate (startDate);
				return CONTENT_URI.BuildUpon ().AppendPath (locationSetting)
					.AppendQueryParameter (COLUMN_DATE, normalizedDate.ToString ()).Build ();
			}

			public static Android.Net.Uri buildWeatherLocationWithDate (String locationSetting, long date)
			{
				return CONTENT_URI.BuildUpon ().AppendPath (locationSetting)
					.AppendQueryParameter(COLUMN_DATE,normalizeDate (date).ToString ()).Build ();
			}

			public static String getLocationSettingFromUri (Android.Net.Uri uri)
			{
				return uri.PathSegments [1];
			}

			public static long getDateFromUri (Android.Net.Uri uri)
			{
				return long.Parse (uri.PathSegments [2]);
			}

			public static long getStartDateFromUri (Android.Net.Uri uri)
			{
				String dateString = uri.GetQueryParameter (COLUMN_DATE);
				if (null != dateString && dateString.Length > 0)
					return long.Parse (dateString);
				else
					return 0;
			}

			public  const string TABLE_NAME = "weather";

			// Column with the foreign key into the location table.
			public  const string COLUMN_LOC_KEY = "location_id";
			// Date, stored as long in milliseconds since the epoch
			public  const string COLUMN_DATE = "date";
			// Weather id as returned by API, to identify the icon to be used
			public  const string COLUMN_WEATHER_id = "weather_id";

			// Short description and long description of the weather, as provided by API.
			// e.g "clear" vs "sky is clear".
			public  const string COLUMN_SHORT_DESC = "short_desc";

			// Min and max temperatures for the day (stored as floats)
			public  const string COLUMN_MIN_TEMP = "min";
			public  const string COLUMN_MAX_TEMP = "max";

			// Humidity is stored as a float representing percentage
			public  const string COLUMN_HUMIDITY = "humidity";

			// Humidity is stored as a float representing percentage
			public  const string COLUMN_PRESSURE = "pressure";

			// Windspeed is stored as a float representing windspeed  mph
			public  const string COLUMN_WIND_SPEED = "wind";

			// Degrees are meteorological degrees (e.g, 0 is north, 180 is south).  Stored as floats.
			public  const string COLUMN_DEGREES = "degrees";
		}
	}
}

