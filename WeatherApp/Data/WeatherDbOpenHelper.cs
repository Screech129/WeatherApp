using System;
using Android.Database.Sqlite;
using Android.Content;
using Android.Util;


namespace WeatherApp
{
	public class WeatherDbOpenHelper:SQLiteOpenHelper
	{
		public const int DATABASE_VERSION = 3;
		public const string DATABASE_NAME = "weather.db";

		public WeatherDbOpenHelper (Context context) :
			base (context, DATABASE_NAME, null, DATABASE_VERSION)
		{
		}


		public override void OnCreate (SQLiteDatabase db)
		{
			const string SQL_CREATE_WEATHER_TABLE = "CREATE TABLE " + WeatherContractOpen.WeatherEntryOpen.TABLE_NAME + " (" +
				// Why AutoIncrement here, and not above?
				// Unique keys will be auto-generated in either case.  But for weather
				// forecasting, it's reasonable to assume the user will want information
				// for a certain date and all dates *following*, so the forecast data
				// should be sorted accordingly.
			                                        WeatherContractOpen.WeatherEntryOpen._ID + " INTEGER PRIMARY KEY AUTOINCREMENT," +

				// the ID of the location entry associated with this weather data
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY + " INTEGER NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " INTEGER NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC + " TEXT NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id + " INTEGER NOT NULL," +

			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP + " REAL NOT NULL, " +

			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES + " REAL NOT NULL, " +

				// Set up the location column as a foreign key to location table.
			                                        " FOREIGN KEY (" + WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY + ") REFERENCES " +
			                                        WeatherContractOpen.LocationEntryOpen.TABLE_NAME + " (" + WeatherContractOpen.LocationEntryOpen._ID + "), " +

				// To assure the application have just one weather entry per day
				// per location, it's created a UNIQUE constraint with REPLACE strategy
			                                        " UNIQUE (" + WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + ", " +
			                                        WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY + ") ON CONFLICT REPLACE);";

			db.ExecSQL (SQL_CREATE_WEATHER_TABLE);

			const string SQL_CREATE_LOCATION_TABLE = "CREATE TABLE " + WeatherContractOpen.LocationEntryOpen.TABLE_NAME + " (" +
			                                         WeatherContractOpen.LocationEntryOpen._ID + " INTEGER PRIMARY KEY AUTOINCREMENT," +
			                                         WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " TEXT UNIQUE NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME + " TEXT NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT + " REAL NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG + " REAL NOT NULL);";
			db.ExecSQL (SQL_CREATE_LOCATION_TABLE);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			// This database is only a cache for online data, so its upgrade policy is
			// to simply to discard the data and start over
			// Note that this only fires if you change the version number for your database.
			// It does NOT depend on the version number for your application.
			// If you want to update the schema without wiping data, commenting out the next 2 lines
			// should be your top priority before modifying this method.
			db.ExecSQL ("DROP TABLE IF EXISTS " + WeatherContractOpen.LocationEntryOpen.TABLE_NAME);
			db.ExecSQL ("DROP TABLE IF EXISTS " + WeatherContractOpen.WeatherEntryOpen.TABLE_NAME);
			OnCreate (db);
		}


	}
}

