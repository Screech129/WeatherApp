using System;

namespace WeatherApp
{
	public class WeatherContractOpen
	{
		public WeatherContractOpen ()
		{
		}

		public   class LocationEntryOpen:BaseColumns
		{
			public  const string TABLE_NAME = "location";
			public const string COLUMN_LOCATION_SETTING = "location_setting";
			public const  string COLUMN_CITY_NAME = "city_name";
			public const  string COLUMN_COORD_LAT = "coord_lat";
			public const  string COLUMN_COORD_LONG = "coord_long";
		}

		/* Inner class that defines the table contents of the weather table */
		public  class WeatherEntryOpen:BaseColumns
		{

			public  const string TABLE_NAME = "weather";

			// Column with the foreign key into the location table.
			public  const string COLUMN_LOC_KEY = "location_id";
			// Date, stored as long in milliseconds since the epoch
			public  const string COLUMN_DATE = "date";
			// Weather id as returned by API, to identify the icon to be used
			public  const string COLUMN_WEATHER_ID = "weather_id";

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

