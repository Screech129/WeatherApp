using System;
using Android.Provider;
using SQLite;

namespace WeatherApp
{
	public static class WeatherContract
	{
		public static long normalizeDate (long startDate)
		{
			// normalize the start date to the beginning of the (UTC) day
			DateTime time = new DateTime ();
			time = DateTime.Parse (startDate.ToString ());
			return long.Parse (time.ToString ());
		}
	}

	/*
        Inner class that defines the table contents of the location table
        Students: This is where you will add the strings.  (Similar to what has been
        done for WeatherEntry)
     */
	[Table ("Location")]
	public class LocationEntry:BaseColumns
	{
		public LocationEntry ()
		{
			
		}

		[Column ("location_setting")]
		[NotNull]
		public string COLUMN_LOCATION_SETTING{ get; set; }

		[Column ("coord_lat")]
		[NotNull]
		public double COLUMN_COORD_LAT{ get; set; }

		[Column ("coord_long")]
		[NotNull]
		public double COLUMN_COORD_LONG{ get; set; }

		[Column ("city_name")]
		[NotNull]
		public string COLUMN_CITY_NAME { get; set; }

	}

	/* Inner class that defines the table contents of the weather table */
	[Table ("Weather")]
	public class WeatherEntry:BaseColumns
	{
		public WeatherEntry ()
		{
			
		}
		// Column with the foreign key into the location table.
		[Column ("location_id")]
		[NotNull]
		[Unique]
		[Indexed (Name = "LocationDate", Order = 2)]
		public int COLUMN_LOC_KEY { get; set; }
		// Date, stored as long in milliseconds since the epoch
		[Column ("date")]
		[NotNull]
		[Unique]
		[Indexed (Name = "LocationDate", Order = 1)]
		public int COLUMN_DATE{ get; set; }
		// Weather id as returned by API, to identify the icon to be used
		[Column ("weather_id")]
		[NotNull]
		public int COLUMN_WEATHER_ID{ get; set; }

		// Short description and long description of the weather, as provided by API.
		// e.g "clear" vs "sky is clear".
		[Column ("short_desc")]
		[NotNull]
		public  String COLUMN_SHORT_DESC { get; set; }

		// Min and max temperatures for the day (stored as floats)
		[Column ("min")]
		[NotNull]
		public  decimal COLUMN_MIN_TEMP{ get; set; }

		[Column ("max")]
		[NotNull]
		public  decimal COLUMN_MAX_TEMP{ get; set; }

		// Humidity is stored as a float representing percentage
		[Column ("humidity")]
		[NotNull]
		public  decimal COLUMN_HUMIDITY{ get; set; }

		// Humidity is stored as a float representing percentage
		[Column ("pressure")]
		[NotNull]
		public  decimal COLUMN_PRESSURE{ get; set; }

		// Windspeed is stored as a float representing windspeed  mph
		[Column ("wind")]
		[NotNull]
		public  decimal COLUMN_WIND_SPEED{ get; set; }

		// Degrees are meteorological degrees (e.g, 0 is north, 180 is south).  Stored as floats.
		[Column ("degrees")]
		[NotNull]
		public  decimal COLUMN_DEGREES{ get; set; }

	}
}

