using System;
using Android.Database.Sqlite;
using Android.Content;
using Android.Util;


namespace WeatherApp
{
	public class WeatherDbOpenHelper:SQLiteOpenHelper
	{
		public const int DatabaseVersion = 3;
		public const string DATABASE_NAME = "weather.db";

		public WeatherDbOpenHelper (Context context) :
			base (context, DATABASE_NAME, null, DatabaseVersion)
		{
		}


		public override void OnCreate (SQLiteDatabase db)
		{
			const string sqlCreateWeatherTable = "CREATE TABLE " + WeatherContractOpen.WeatherEntryOpen.TableName + " (" +
				// Why AutoIncrement here, and not above?
				// Unique keys will be auto-generated in either case.  But for weather
				// forecasting, it's reasonable to assume the user will want information
				// for a certain date and all dates *following*, so the forecast data
				// should be sorted accordingly.
			                                        WeatherContractOpen.WeatherEntryOpen.Id + " INTEGER PRIMARY KEY AUTOINCREMENT," +

				// the ID of the location entry associated with this weather data
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnLocKey + " INTEGER NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnDate + " INTEGER NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc + " TEXT NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId + " INTEGER NOT NULL," +

			                                        WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp + " REAL NOT NULL, " +

			                                        WeatherContractOpen.WeatherEntryOpen.ColumnHumidity + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnPressure + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnWindSpeed + " REAL NOT NULL, " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnDegrees + " REAL NOT NULL, " +

				// Set up the location column as a foreign key to location table.
			                                        " FOREIGN KEY (" + WeatherContractOpen.WeatherEntryOpen.ColumnLocKey + ") REFERENCES " +
			                                        WeatherContractOpen.LocationEntryOpen.TableName + " (" + WeatherContractOpen.LocationEntryOpen.Id + "), " +

				// To assure the application have just one weather entry per day
				// per location, it's created a UNIQUE constraint with REPLACE strategy
			                                        " UNIQUE (" + WeatherContractOpen.WeatherEntryOpen.ColumnDate + ", " +
			                                        WeatherContractOpen.WeatherEntryOpen.ColumnLocKey + ") ON CONFLICT REPLACE);";

			db.ExecSQL (sqlCreateWeatherTable);

			const string sqlCreateLocationTable = "CREATE TABLE " + WeatherContractOpen.LocationEntryOpen.TableName + " (" +
			                                         WeatherContractOpen.LocationEntryOpen.Id + " INTEGER PRIMARY KEY AUTOINCREMENT," +
			                                         WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting + " TEXT UNIQUE NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.ColumnCityName + " TEXT NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.ColumnCoordLat + " REAL NOT NULL, " +
			                                         WeatherContractOpen.LocationEntryOpen.ColumnCoordLong + " REAL NOT NULL);";
			db.ExecSQL (sqlCreateLocationTable);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			// This database is only a cache for online data, so its upgrade policy is
			// to simply to discard the data and start over
			// Note that this only fires if you change the version number for your database.
			// It does NOT depend on the version number for your application.
			// If you want to update the schema without wiping data, commenting out the next 2 lines
			// should be your top priority before modifying this method.
			db.ExecSQL ("DROP TABLE IF EXISTS " + WeatherContractOpen.LocationEntryOpen.TableName);
			db.ExecSQL ("DROP TABLE IF EXISTS " + WeatherContractOpen.WeatherEntryOpen.TableName);
			OnCreate (db);
		}


	}
}

