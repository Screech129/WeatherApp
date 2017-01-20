using System;
using Android.Provider;
using System.Collections.Generic;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace WeatherApp
{
	public static class WeatherContract
	{
		public static long NormalizeDate (long startDate)
		{
			// normalize the start date to the beginning of the (UTC) day
			var time = new DateTime ();
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
		public string LocationSetting{ get; set; }

		[NotNull]
		public double CoordLat{ get; set; }

		[NotNull]
		public double CoordLong{ get; set; }

		[NotNull]
		public string CityName { get; set; }

		[OneToMany]
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
		public int Date{ get; set; }
		// Weather id as returned by API, to identify the icon to be used
		[NotNull]
		public int WeatherId{ get; set; }

		// Short description and long description of the weather, as provided by API.
		// e.g "clear" vs "sky is clear".
		[NotNull]
		public  String ShortDesc { get; set; }

		// Min and max temperatures for the day (stored as floats)
		[NotNull]
		public  decimal MinTemp{ get; set; }

		[NotNull]
		public  decimal MaxTemp{ get; set; }

		// Humidity is stored as a float representing percentage
		[NotNull]
		public  decimal Humidity{ get; set; }

		// Humidity is stored as a float representing percentage
		[NotNull]
		public  decimal Pressure{ get; set; }

		// Windspeed is stored as a float representing windspeed  mph
		[NotNull]
		public  decimal Wind{ get; set; }

		// Degrees are meteorological degrees (e.g, 0 is north, 180 is south).  Stored as floats.
		[NotNull]
		public  decimal Degrees{ get; set; }

		[ManyToOne]
		public LocationEntry LocationEntry{ get; set; }

	}
}

