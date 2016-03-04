using System;
using NUnit.Framework;
using Android.Content;
using WeatherApp;
using Android.Database;
using Android.Database.Sqlite;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace AndroidTest
{
	[TestFixture]
	public class TestProvider
	{
		public TestProvider ()
		{
		}

		Context context = Android.App.Application.Context;
		ICursor cursor;

		[SetUp]
		public void Setup ()
		{
			DeleteAllRecords ();

		}


		[TearDown]
		public void Tear ()
		{
		}

		/*
       This helper function Deletes all records from both database tables using the ContentProvider.
       It also queries the ContentProvider to make sure that the database has been successfully
       Deleted, so it cannot be used until the Query and Delete functions have been written
       in the ContentProvider.

       Students: Replace the calls to DeleteAllRecordsFromDB with this one after you have written
       the Delete functionality in the ContentProvider.
     */

		public void DeleteAllRecordsFromProvider ()
		{
			context.ContentResolver.Delete (
				WeatherContractOpen.WeatherEntryOpen.CONTENT_URI,
				null,
				null
			);
			context.ContentResolver.Delete (
				WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				null,
				null
			);

			cursor = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.CONTENT_URI,
				null,
				null,
				null,
				null
			);
			Assert.AreEqual (0, cursor.Count, "Error: Records not Deleted from Weather table during Delete");
			cursor.Close ();

			cursor = context.ContentResolver.Query (
				WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				null,
				null,
				null,
				null
			);
			Assert.AreEqual (0, cursor.Count, "Error: Records not Deleted from Location table during Delete");
			cursor.Close ();
		}

		/*
       This helper function Deletes all records from both database tables using the database
       functions only.  This is designed to be used to reset the state of the database until the
       Delete functionality is available in the ContentProvider.
     */
		public void DeleteAllRecordsFromDB ()
		{
			var dbHelper = new WeatherDbOpenHelper (context);
			SQLiteDatabase db = dbHelper.WritableDatabase;

			db.Delete (WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, null, null);
			db.Delete (WeatherContractOpen.LocationEntryOpen.TABLE_NAME, null, null);
			db.Close ();
		}

		/*
        Student: Refactor this function to use the DeleteAllRecordsFromProvider functionality once
        you have implemented Delete functionality there.
     */
		public void DeleteAllRecords ()
		{
			DeleteAllRecordsFromDB ();
		}

		// Since we want each test to start with a clean slate, run DeleteAllRecords
		// in setUp (called by the test runner before each test).

		/*
        This test checks to make sure that the content provider is registered correctly.
        Students: Uncomment this test to make sure you've correctly registered the WeatherProvider.
     */
		//		[Test]
		//		public void testProviderRegistry ()
		//		{
		//			PackageManager pm = context.PackageManager;
		//			var className = typeof(WeatherProvider).FullName;
		//			// We define the component name based on the package name from the context and the
		//			// WeatherProvider class.
		//			ComponentName componentName = new ComponentName ("WeatherApp",
		//				                              className);
		//			try {
		//				// Fetch the provider info using the component name from the PackageManager
		//				// This throws an exception if the provider isn't registered.
		//				var test = componentName.ClassName;
		//				ProviderInfo providerInfo = pm.GetProviderInfo (componentName, 0);
		//
		//				// Make sure that the registered authority matches the authority from the Contract.
		//				Assert.AreEqual (providerInfo.Authority, WeatherContractOpen.CONTENT_AUTHORITY,
		//					"Error: WeatherProvider registered with authority: " + providerInfo.Authority + " instead of authority: " + WeatherContractOpen.CONTENT_AUTHORITY);
		//			} catch (PackageManager.NameNotFoundException e) {
		//				// I guess the provider isn't registered correctly.
		//				Assert.IsTrue (false, e.Message);
		//			}
		//		}

		/*
            This test doesn't touch the database.  It verifies that the ContentProvider returns
            the correct type for each type of URI that it can handle.
            Students: Uncomment this test to verify that your implementation of GetType is
            functioning correctly.
         */
		[Test]
		public void testGetType ()
		{
			// content://com.example.android.sunshine.app/weather/
			String type = context.ContentResolver.GetType (WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_URI);
			// vnd.android.cursor.dir/com.example.android.sunshine.app/weather
			Assert.AreEqual (WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_TYPE, type, "Error: the WeatherEntry CONTENT_URI should return WeatherEntry.CONTENT_TYPE");
		
			String testLocation = "94074";
			// content://com.example.android.sunshine.app/weather/94074
			type = context.ContentResolver.GetType (
				WeatherApp.WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (testLocation));
			// vnd.android.cursor.dir/com.example.android.sunshine.app/weather
			Assert.AreEqual (WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_TYPE, type, "Error: the WeatherEntry CONTENT_URI with location should return WeatherEntry.CONTENT_TYPE");
		
			long testDate = 1419120000L; // December 21st, 2014
			// content://com.example.android.sunshine.app/weather/94074/20140612
			type = context.ContentResolver.GetType (
				WeatherApp.WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (testLocation, testDate));
			// vnd.android.cursor.item/com.example.android.sunshine.app/weather/1419120000
			Assert.AreEqual (WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_ITEM_TYPE, type, "Error: the WeatherEntry CONTENT_URI with location and date should return WeatherEntry.CONTENT_ITEM_TYPE");
		
			// content://com.example.android.sunshine.app/location/
			type = context.ContentResolver.GetType (WeatherApp.WeatherContractOpen.LocationEntryOpen.CONTENT_URI);
			// vnd.android.cursor.dir/com.example.android.sunshine.app/location
			Assert.AreEqual (WeatherApp.WeatherContractOpen.LocationEntryOpen.CONTENT_TYPE, type, "Error: the LocationEntry CONTENT_URI should return LocationEntry.CONTENT_TYPE");
		}


		/*
        This test uses the database directly to insert and then uses the ContentProvider to
        read out the data.  Uncomment this test to see if the basic weather Query functionality
        given in the ContentProvider is working correctly.
     */
		[Test]
		public void testBasicWeatherQuery ()
		{
			// insert our test records into the database
			var dbHelper = new WeatherDbOpenHelper (context);
			var db = dbHelper.WritableDatabase;
		
			ContentValues testValues = TestUtilitiesOpen.createNorthPoleLocationValues ();
			long locationRowId = TestUtilitiesOpen.insertNorthPoleLocationValues (context);
		
			// Fantastic.  Now that we have a location, add some weather!
			ContentValues weatherValues = TestUtilitiesOpen.createWeatherValues (locationRowId);
		
			long weatherRowId = db.Insert (WeatherApp.WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, null, weatherValues);
			Assert.IsTrue (weatherRowId != -1, "Unable to Insert WeatherEntry into the Database");
		
			db.Close ();
		
			// Test the basic content provider Query
			var weather = context.ContentResolver.Query (
				              WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_URI,
				              null,
				              null,
				              null,
				              null
			              );
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			// Make sure we get the correct cursor out of the database
			TestUtilitiesOpen.validateCurrentRecord ("testBasicWeatherQuery", weather, weatherValues);
		}

		/*
        This test uses the database directly to insert and then uses the ContentProvider to
        read out the data.  Uncomment this test to see if your location queries are
        performing correctly.
     */
		[Test]
		public void testBasicLocationQueries ()
		{
			// insert our test records into the database
			var dbHelper = new WeatherDbOpenHelper (context);
			SQLiteDatabase db = dbHelper.WritableDatabase;
		
			ContentValues testValues = TestUtilitiesOpen.createNorthPoleLocationValues ();
			long locationRowId = TestUtilitiesOpen.insertNorthPoleLocationValues (context);
		
			// Test the basic content provider Query
			var location = context.ContentResolver.Query (
				               WeatherApp.WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				               null,
				               null,
				               null,
				               null
			               );
			Assert.IsTrue (location.MoveToFirst (), "Cursor is empty");
		
			// Make sure we get the correct cursor out of the database
			TestUtilitiesOpen.validateCurrentRecord ("testBasicLocationQueries, location Query", location, testValues);
		
			// Has the NotificationUri been set correctly? --- we can only test this easily against API
			// level 19 or greater because getNotificationUri was added in API level 19.
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat) {
				Assert.AreEqual (location.NotificationUri.ToString (), WeatherApp.WeatherContractOpen.LocationEntryOpen.CONTENT_URI.ToString (), "Error: Location Query did not properly set NotificationUri");
			}
		}

		/*
        This test uses the provider to insert and then update the data. Uncomment this test to
        see if your update location is functioning correctly.
     */
	
		public void testUpdateLocation ()
		{
			const string LOG_TAG = "Provider Test";

			// Create a new map of values, where column names are the keys
			ContentValues values = TestUtilitiesOpen.createNorthPoleLocationValues ();
		
			Android.Net.Uri locationUri = context.ContentResolver.
		                Insert (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, values);
			long locationRowId = ContentUris.ParseId (locationUri);
		
			// Verify we got a row back.
			Assert.IsTrue (locationRowId != -1);
			Log.Debug (LOG_TAG, "New row id: " + locationRowId);
		
			ContentValues updatedValues = new ContentValues (values);
			updatedValues.Put (WeatherContractOpen.LocationEntryOpen._ID, locationRowId);
			updatedValues.Put (WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME, "Santa's Village");
		
			// Create a cursor with observer to make sure that the content provider is notifying
			// the observers as expected
			var location = context.ContentResolver.Query (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, null, null, null, null);
		
			TestUtilitiesOpen.TestContentObserver tco = TestUtilitiesOpen.getTestContentObserver ();
			location.RegisterContentObserver (tco);
		
			int count = context.ContentResolver.Update (
				            WeatherContractOpen.LocationEntryOpen.CONTENT_URI, updatedValues, WeatherContractOpen.LocationEntryOpen._ID + "= ?",
				            new String[] { locationRowId.ToString () });
			Assert.AreEqual (count, 1);
		
			// Test to make sure our observer is called.  If not, we throw an assertion.
			//
			// Students: If your code is failing here, it means that your content provider
			// isn't calling getContext().ContentResolver.notifyChange(uri, null);
			tco.waitForNotificationOrFail ();
		
			location.UnregisterContentObserver (tco);
			location.Close ();
		
			// A cursor is your primary interface to the Query results.
			cursor = context.ContentResolver.Query (
				WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				null,   // projection
				WeatherContractOpen.LocationEntryOpen._ID + " = " + locationRowId,
				null,   // Values for the "where" clause
				null    // sort order
			);
		
			Assert.IsTrue (location.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testUpdateLocation.  Error validating location entry update.",
				cursor, updatedValues);
		
			cursor.Close ();
		}


		// Make sure we can still Delete after adding/updating stuff
		//
		// Student: Uncomment this test after you have completed writing the insert functionality
		// in your provider.  It relies on insertions with testInsertReadProvider, so insert and
		// Query functionality must also be complete before this test can be used.
		[Test]
		public void testInsertReadProvider ()
		{
			ContentValues testValues = TestUtilitiesOpen.createNorthPoleLocationValues ();
		
			// Register a content observer for our insert.  This time, directly with the content resolver
			TestUtilitiesOpen.TestContentObserver tco = TestUtilitiesOpen.getTestContentObserver ();
			context.ContentResolver.RegisterContentObserver (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, true, tco);
			Android.Net.Uri locationUri = context.ContentResolver.Insert (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, testValues);
		
			// Did our content observer get called?  Students:  If this fails, your insert location
			// isn't calling getContext().getContentResolver().notifyChange(uri, null);
			tco.waitForNotificationOrFail ();
			context.ContentResolver.UnregisterContentObserver (tco);
		
			long locationRowId = ContentUris.ParseId (locationUri);
		
			// Verify we got a row back.
			Assert.IsTrue (locationRowId != -1);
		
			// Data's inserted.  IN THEORY.  Now pull some out to stare at it and verify it made
			// the round trip.
		
			// A cursor is your primary interface to the Query results.
			var weather = context.ContentResolver.Query (
				              WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				              null, // leaving "columns" null just returns all the columns.
				              null, // cols for "where" clause
				              null, // values for "where" clause
				              null  // sort order
			              );
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testInsertReadProvider. Error validating WeatherContractOpen.LocationEntryOpen.",
				weather, testValues);
		
			// Fantastic.  Now that we have a location, add some weather!
			ContentValues weatherValues = TestUtilitiesOpen.createWeatherValues (locationRowId);
			// The TestContentObserver is a one-shot class
			tco = TestUtilitiesOpen.getTestContentObserver ();
		
			context.ContentResolver.RegisterContentObserver (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, true, tco);
		
			Android.Net.Uri weatherInsertUri = context.ContentResolver
		                .Insert (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, weatherValues);
			Assert.IsTrue (weatherInsertUri != null);
		
			// Did our content observer get called?  Students:  If this fails, your insert weather
			// in your ContentProvider isn't calling
			// getContext().getContentResolver().notifyChange(uri, null);
			tco.waitForNotificationOrFail ();
			context.ContentResolver.UnregisterContentObserver (tco);
		
			// A cursor is your primary interface to the Query results.
			weather = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.CONTENT_URI,  // Table to Query
				null, // leaving "columns" null just returns all the columns.
				null, // cols for "where" clause
				null, // values for "where" clause
				null // columns to group by
			);
			
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testInsertReadProvider. Error validating WeatherContractOpen.WeatherEntryOpen insert.",
				weather, weatherValues);
		
			// Add the location values in with the weather data so that we can make
			// sure that the join worked and we actually get all the values back
			weatherValues.PutAll (testValues);
			var test = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (TestUtilitiesOpen.TEST_LOCATION);
			// Get the joined Weather and Location data
			weather = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (TestUtilitiesOpen.TEST_LOCATION),
				null, // leaving "columns" null just returns all the columns.
				null, // cols for "where" clause
				null, // values for "where" clause
				null  // sort order
			);
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testInsertReadProvider.  Error validating joined Weather and Location Data.",
				weather, weatherValues);
		
			// Get the joined Weather and Location data with a start date
			weather = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithStartDate (
					TestUtilitiesOpen.TEST_LOCATION, TestUtilitiesOpen.TEST_DATE),
				null, // leaving "columns" null just returns all the columns.
				null, // cols for "where" clause
				null, // values for "where" clause
				null  // sort order
			);
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testInsertReadProvider.  Error validating joined Weather and Location Data with start date.",
				weather, weatherValues);
		
			// Get the joined Weather data for a specific date
			weather = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (TestUtilitiesOpen.TEST_LOCATION, TestUtilitiesOpen.TEST_DATE),
				null,
				null,
				null,
				null
			);
			Assert.IsTrue (weather.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testInsertReadProvider.  Error validating joined Weather and Location data for a specific date.",
				weather, weatherValues);
		}

		// Make sure we can still Delete after adding/updating stuff
		//
		// Student: Uncomment this test after you have completed writing the Delete functionality
		// in your provider.  It relies on insertions with testInsertReadProvider, so insert and
		// Query functionality must also be complete before this test can be used.
		[Test]
		public void testDeleteRecords ()
		{
			testInsertReadProvider ();
		
			// Register a content observer for our location Delete.
			TestUtilitiesOpen.TestContentObserver locationObserver = TestUtilitiesOpen.getTestContentObserver ();
			context.ContentResolver.RegisterContentObserver (WeatherApp.WeatherContractOpen.LocationEntryOpen.CONTENT_URI, true, locationObserver);
		
			// Register a content observer for our weather Delete.
			TestUtilitiesOpen.TestContentObserver weatherObserver = TestUtilitiesOpen.getTestContentObserver ();
			context.ContentResolver.RegisterContentObserver (WeatherApp.WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, true, weatherObserver);
		
			DeleteAllRecordsFromProvider ();
		
			// Students: If either of these fail, you most-likely are not calling the
			// getContext().getContentResolver().notifyChange(uri, null); in the ContentProvider
			// Delete.  (only if the insertReadProvider is succeeding)
			locationObserver.waitForNotificationOrFail ();
			weatherObserver.waitForNotificationOrFail ();
		
			context.ContentResolver.UnregisterContentObserver (locationObserver);
			context.ContentResolver.UnregisterContentObserver (weatherObserver);
		}


		private const int BULK_INSERT_RECORDS_TO_INSERT = 10;

		ContentValues[] createBulkInsertWeatherValues (long locationRowId)
		{
			long currentTestDate = TestUtilitiesOpen.TEST_DATE;
			long millisecondsInADay = 1000 * 60 * 60 * 24;
			ContentValues[] returnContentValues = new ContentValues[BULK_INSERT_RECORDS_TO_INSERT];

			for (int i = 0; i < BULK_INSERT_RECORDS_TO_INSERT; i++, currentTestDate += millisecondsInADay) {
				ContentValues weatherValues = new ContentValues ();
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY, locationRowId);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE, currentTestDate);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES, 1.1);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY, 1.2 + 0.01 * (float)i);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE, 1.3 - 0.01 * (float)i);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP, 75 + i);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP, 65 - i);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC, "Asteroids");
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED, 5.5 + 0.2 * (float)i);
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id, 321);
				returnContentValues [i] = weatherValues;
			}
			return returnContentValues;
		}

		// Student: Uncomment this test after you have completed writing the BulkInsert functionality
		// in your provider.  Note that this test will work with the built-in (default) provider
		// implementation, which just inserts records one-at-a-time, so really do implement the
		// BulkInsert ContentProvider function.
		[Test]
		public void testBulkInsert ()
		{
			// first, let's create a location value
			ContentValues testValues = TestUtilitiesOpen.createNorthPoleLocationValues ();
			Android.Net.Uri locationUri = context.ContentResolver.Insert (WeatherContractOpen.LocationEntryOpen.CONTENT_URI, testValues);
			long locationRowId = ContentUris.ParseId (locationUri);
		
			// Verify we got a row back.
			Assert.IsTrue (locationRowId != -1);
		
			// Data's inserted.  IN THEORY.  Now pull some out to stare at it and verify it made
			// the round trip.
		
			// A cursor is your primary interface to the Query results.
			cursor = context.ContentResolver.Query (
				WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				null, // leaving "columns" null just returns all the columns.
				null, // cols for "where" clause
				null, // values for "where" clause
				null  // sort order
			);
		
			Assert.IsTrue (cursor.MoveToFirst (), "Cursor is empty");
			TestUtilitiesOpen.validateCurrentRecord ("testBulkInsert. Error validating WeatherContractOpen.LocationEntryOpen.",
				cursor, testValues);
		
			// Now we can bulkInsert some weather.  In fact, we only implement BulkInsert for weather
			// entries.  With ContentProviders, you really only have to implement the features you
			// use, after all.
			ContentValues[] bulkInsertContentValues = createBulkInsertWeatherValues (locationRowId);
		
			// Register a content observer for our bulk insert.
			TestUtilitiesOpen.TestContentObserver weatherObserver = TestUtilitiesOpen.getTestContentObserver ();
			context.ContentResolver.RegisterContentObserver (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, true, weatherObserver);
		
			int insertCount = context.ContentResolver.BulkInsert (WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, bulkInsertContentValues);
		
			// Students:  If this fails, it means that you most-likely are not calling the
			// getContext().ContentResolver.notifyChange(uri, null); in your BulkInsert
			// ContentProvider method.
			weatherObserver.waitForNotificationOrFail ();
			context.ContentResolver.UnregisterContentObserver (weatherObserver);
		
			Assert.AreEqual (insertCount, BULK_INSERT_RECORDS_TO_INSERT);
		
			// A cursor is your primary interface to the Query results.
			cursor = context.ContentResolver.Query (
				WeatherContractOpen.WeatherEntryOpen.CONTENT_URI,
				null, // leaving "columns" null just returns all the columns.
				null, // cols for "where" clause
				null, // values for "where" clause
				WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " ASC"  // sort order == by DATE ASCENDING
			);
		
			// we should have as many records in the database as we've inserted
			Assert.AreEqual (cursor.Count, BULK_INSERT_RECORDS_TO_INSERT);
		
			// and let's make sure they match the ones we created
			cursor.MoveToFirst ();
			for (int i = 0; i < BULK_INSERT_RECORDS_TO_INSERT; i++, cursor.MoveToNext ()) {
				TestUtilitiesOpen.validateCurrentRecord ("testBulkInsert.  Error validating WeatherContractOpen.WeatherEntryOpen " + i,
					cursor, bulkInsertContentValues [i]);
			}
			cursor.Close ();
		}
	}
}

