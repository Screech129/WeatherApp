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




		public static void validateCurrentRecord (String error, LocationEntry expectedValues)
		{
			var test = expectedValues;
//			
//			foreach (PropertyInfo entry in properties) {
//				String columnName = entry.ToString ();
//				int idx = valueCursor.GetColumnIndex (columnName);
//				Assert.IsFalse (idx == -1, "Column '" + columnName + "' not found. " + error);
//				String expectedValue = expectedValues [entry].ToString ();
//				Assert.AreEqual ("Value '" + expectedValues.Get (entry) +
//				"' did not match the expected value '" +
//				expectedValue + "'. " + error, expectedValue, valueCursor.GetString (idx));
//			}
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
				COLUMN_LOCATION_SETTING = TEST_LOCATION,
				COLUMN_CITY_NAME = "North Pole",
				COLUMN_COORD_LAT = 64.7488,
				COLUMN_COORD_LONG = -147.353
			};
			LocationEntry locEntry2 = new LocationEntry () {
				COLUMN_LOCATION_SETTING = TEST_LOCATION,
				COLUMN_CITY_NAME = "Your Mom's",
				COLUMN_COORD_LAT = 61.345,
				COLUMN_COORD_LONG = -180.353
			};
			LocationEntry locEntry3 = new LocationEntry () {
				COLUMN_LOCATION_SETTING = TEST_LOCATION,
				COLUMN_CITY_NAME = "Cartel HQ",
				COLUMN_COORD_LAT = -45.7488,
				COLUMN_COORD_LONG = -101.353
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

