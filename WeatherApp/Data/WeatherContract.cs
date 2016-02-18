using System;
using Android.Provider;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

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
	public class LocationEntry
	{
		public LocationEntry ()
		{
			
		}

		[PrimaryKey,AutoIncrement]
		public int LocationId{ get; set; }

		[NotNull]
		public string location_setting{ get; set; }

		[NotNull]
		public double coord_lat{ get; set; }

		[NotNull]
		public double coord_long{ get; set; }

		[NotNull]
		public string city_name { get; set; }

		[OneToMany ("WeatherRecordId")]
		public List<WeatherEntry> WeatherEntries{ get; set; }

	}

	/* Inner class that defines the table contents of the weather table */
	[Table ("Weather")]
	public class WeatherEntry
	{
		public WeatherEntry ()
		{
			
		}

		[PrimaryKey,AutoIncrement]
		public int WeatherRecordId{ get; set; }
		// Column with the foreign key into the location table.
		[ForeignKey (typeof(LocationEntry))]
		public int LocationId { get; set; }
		// Date, stored as long in milliseconds since the epoch
		[NotNull]
		[Unique]
		[Indexed (Name = "LocationDate", Order = 1)]
		public int date{ get; set; }
		// Weather id as returned by API, to identify the icon to be used
		[NotNull]
		public int weather_id{ get; set; }

		// Short description and long description of the weather, as provided by API.
		// e.g "clear" vs "sky is clear".
		[NotNull]
		public  String short_desc { get; set; }

		// Min and max temperatures for the day (stored as floats)
		[NotNull]
		public  decimal min_temp{ get; set; }

		[NotNull]
		public  decimal max_temp{ get; set; }

		// Humidity is stored as a float representing percentage
		[NotNull]
		public  decimal humidity{ get; set; }

		// Humidity is stored as a float representing percentage
		[NotNull]
		public  decimal pressure{ get; set; }

		// Windspeed is stored as a float representing windspeed  mph
		[NotNull]
		public  decimal wind{ get; set; }

		// Degrees are meteorological degrees (e.g, 0 is north, 180 is south).  Stored as floats.
		[NotNull]
		public  decimal degrees{ get; set; }

		//		[ManyToOne]
		//		public LocationEntry LocationEntry{ get; set; }

	}
}

