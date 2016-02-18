using System;
using NUnit.Framework;
using Android.Database.Sqlite;
using System.Collections.Generic;
using Android.Content;
using System.Linq;
using Android.Database;
using Android.OS;
using System.Threading.Tasks;
using WeatherApp;
using SQLite;
using System.IO;
using System.Reflection;


namespace AndroidTest
{
	[TestFixture]
	public class TestUtilities
	{
		[SetUp]
		public void Setup ()
		{
		}


		[TearDown]
		public void Tear ()
		{
		}


		const string TEST_LOCATION = "99705";
		const long TEST_DATE = 1419033600L;
		// December 20th, 2014
		const String DATABASE_NAME = "weather.db";
		static string personalFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
		static readonly string DATABASE_PATH = Path.Combine (personalFolder, DATABASE_NAME);
		SQLiteConnection con = new SQLiteConnection (DATABASE_PATH);




		public static bool validateCurrentRecord (LocationEntry expectedValues)
		{
			Assert.IsTrue (expectedValues.LocationId > 0, "Error Row not inserted correctly");
			return true;
		}

		public static bool validateCurrentRecordWeath (WeatherEntry expectedValues)
		{
			Assert.IsTrue (expectedValues.WeatherRecordId > 0, "Error Row not inserted correctly");
			return true;
		}

		static ContentValues createWeatherValues (long locationRowId)
		{
			ContentValues weatherValues = new ContentValues ();
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_LOC_KEY, locationRowId);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_DATE, TEST_DATE);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_DEGREES, 1.1);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_HUMIDITY, 1.2);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_PRESSURE, 1.3);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_MAX_TEMP, 75);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_MIN_TEMP, 65);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_SHORT_DESC, "Asteroids");
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_WIND_SPEED, 5.5);
//			weatherValues.Put (WeatherContract.WeatherEntry.COLUMN_WEATHER_ID, 321);

			return weatherValues;
		}


		/*
        Students: You can uncomment this helper function once you have finished creating the
        LocationEntry part of the WeatherContract.
     */
		static List<LocationEntry> createNorthPoleLocationValues ()
		{
			// Create a new map of values, where column names are the keys
			List<LocationEntry> locEntries = new List<LocationEntry> ();

			LocationEntry locEntry1 = new LocationEntry () {
				location_setting = TEST_LOCATION,
				city_name = "North Pole",
				coord_lat = 64.7488,
				coord_long = -147.353
			};
			LocationEntry locEntry2 = new LocationEntry () {
				location_setting = TEST_LOCATION,
				city_name = "Your Mom's",
				coord_lat = 61.345,
				coord_long = -180.353
			};
			LocationEntry locEntry3 = new LocationEntry () {
				location_setting = TEST_LOCATION,
				city_name = "Cartel HQ",
				coord_lat = -45.7488,
				coord_long = -101.353
			};
			locEntries.Add (locEntry1);
			locEntries.Add (locEntry2);
			locEntries.Add (locEntry3);

			return locEntries;
		}


		static public long insertNorthPoleLocationValues ()
		{
			// insert our test records into the database
			WeatherDbHelper dbHelper = new WeatherDbHelper ();
			List<LocationEntry> testLocation = TestUtilities.createNorthPoleLocationValues ();
		
			long locationRowId = -1;

			foreach (var loc in testLocation) {
				
				locationRowId = dbHelper.Insert (loc);
			}
		
			return locationRowId;
		}

		static List<WeatherEntry> createWeatherValues ()
		{
			// Create a new map of values, where column names are the keys
			List<WeatherEntry> weathEntries = new List<WeatherEntry> ();
			var date = 0;
			int.TryParse (DateTime.Now.ToString (), out date);
			WeatherEntry weathEntry1 = new WeatherEntry () {
				date = date,
				degrees = 75,
				humidity = 45,
				LocationId = 1,
				max_temp = 80,
				min_temp = 56,
				pressure = 32,
				short_desc = "Warmish",
				weather_id = 23,
				wind = 5
				
				
				
			};
			WeatherEntry weathEntry2 = new WeatherEntry () {
				date = date,
				degrees = 72,
				humidity = 42,
				LocationId = 1,
				max_temp = 82,
				min_temp = 51,
				pressure = 34,
				short_desc = "Warm Kinda",
				weather_id = 22,
				wind = 45
			};
			WeatherEntry weathEntry3 = new WeatherEntry () {
				date = date,
				degrees = 7,
				humidity = 4,
				LocationId = 1,
				max_temp = 80,
				min_temp = 56,
				pressure = 32,
				short_desc = "Impossible",
				weather_id = 23,
				wind = 5
			};
			weathEntries.Add (weathEntry1);
			weathEntries.Add (weathEntry2);
			weathEntries.Add (weathEntry3);

			return weathEntries;
		}

		static public long insertFakeWeather ()
		{
			// insert our test records into the database
			WeatherDbHelper dbHelper = new WeatherDbHelper ();
			List<WeatherEntry> testWeather = TestUtilities.createWeatherValues ();

			long weatherRowId = -1;

			foreach (var weath in testWeather) {

				weatherRowId = dbHelper.Insert (weath);
			}

			return weatherRowId;
		}

		/*
        Students: The functions we provide inside of TestProvider use this utility class to test
        the ContentObserver callbacks using the PollingCheck class that we grabbed from the Android
        CTS tests.
        Note that this only tests that the onChange function is called; it does not test that the
        correct Uri is returned.
     */
	
		class TestContentObserver:ContentObserver
		{
			HandlerThread mHT;
			bool ContentChanged = false;

			public static TestContentObserver getTestContentObserver ()
			{
				HandlerThread ht = new HandlerThread ("ContentObserverThread");
				ht.Start ();
				return new TestContentObserver (ht);
			}

			private TestContentObserver (HandlerThread ht) : base (new Handler (ht.Looper))
			{
				mHT = ht;
			}

			// On earlier versions of Android, this onChange method is called
			public override void OnChange (bool selfChange)
			{
				base.OnChange (selfChange, null);
			}

			public override void OnChange (bool selfChange, Android.Net.Uri uri)
			{
				base.OnChange (selfChange, uri);
			}


			public async Task waitForNotificationOrFail ()
			{
				// Note: The PollingCheck class is taken from the Android CTS (Compatibility Test Suite).
				// It's useful to look at the Android CTS source for ideas on how to test your Android
				// applications.  The reason that PollingCheck works is that, by default, the JUnit
				// testing framework is not running on the main Android application thread.
				while (!this.ContentChanged) {
					await Task.Delay (5000);
				}



				mHT.Quit ();
			}



	
		}

		static TestContentObserver getTestContentObserver ()
		{
			return TestContentObserver.getTestContentObserver ();
		}
	}
}

