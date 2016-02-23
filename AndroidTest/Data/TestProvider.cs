using System;
using NUnit.Framework;
using Android.Content;
using WeatherApp;
using Android.Database;
using Android.Database.Sqlite;

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
		//    public void testProviderRegistry() {
		//        PackageManager pm = context.getPackageManager();
		//
		//        // We define the component name based on the package name from the context and the
		//        // WeatherProvider class.
		//        ComponentName componentName = new ComponentName(context.getPackageName(),
		//                WeatherProvider.class.getName());
		//        try {
		//            // Fetch the provider info using the component name from the PackageManager
		//            // This throws an exception if the provider isn't registered.
		//            ProviderInfo providerInfo = pm.getProviderInfo(componentName, 0);
		//
		//            // Make sure that the registered authority matches the authority from the Contract.
		//            Assert.AreEqual("Error: WeatherProvider registered with authority: " + providerInfo.authority +
		//                    " instead of authority: " + WeatherContract.CONTENT_AUTHORITY,
		//                    providerInfo.authority, WeatherContract.CONTENT_AUTHORITY);
		//        } catch (PackageManager.NameNotFoundException e) {
		//            // I guess the provider isn't registered correctly.
		//            assertTrue("Error: WeatherProvider not registered at " + context.getPackageName(),
		//                    false);
		//        }
		//    }

		/*
            This test doesn't touch the database.  It verifies that the ContentProvider returns
            the correct type for each type of URI that it can handle.
            Students: Uncomment this test to verify that your implementation of GetType is
            functioning correctly.
         */
		//    public void testGetType() {
		//        // content://com.example.android.sunshine.app/weather/
		//        String type = context.getContentResolver().getType(WeatherEntry.CONTENT_URI);
		//        // vnd.android.cursor.dir/com.example.android.sunshine.app/weather
		//        Assert.AreEqual("Error: the WeatherEntry CONTENT_URI should return WeatherEntry.CONTENT_TYPE",
		//                WeatherEntry.CONTENT_TYPE, type);
		//
		//        String testLocation = "94074";
		//        // content://com.example.android.sunshine.app/weather/94074
		//        type = context.getContentResolver().getType(
		//                WeatherEntry.buildWeatherLocation(testLocation));
		//        // vnd.android.cursor.dir/com.example.android.sunshine.app/weather
		//        Assert.AreEqual("Error: the WeatherEntry CONTENT_URI with location should return WeatherEntry.CONTENT_TYPE",
		//                WeatherEntry.CONTENT_TYPE, type);
		//
		//        long testDate = 1419120000L; // December 21st, 2014
		//        // content://com.example.android.sunshine.app/weather/94074/20140612
		//        type = context.getContentResolver().getType(
		//                WeatherEntry.buildWeatherLocationWithDate(testLocation, testDate));
		//        // vnd.android.cursor.item/com.example.android.sunshine.app/weather/1419120000
		//        Assert.AreEqual("Error: the WeatherEntry CONTENT_URI with location and date should return WeatherEntry.CONTENT_ITEM_TYPE",
		//                WeatherEntry.CONTENT_ITEM_TYPE, type);
		//
		//        // content://com.example.android.sunshine.app/location/
		//        type = context.getContentResolver().getType(LocationEntry.CONTENT_URI);
		//        // vnd.android.cursor.dir/com.example.android.sunshine.app/location
		//        Assert.AreEqual("Error: the LocationEntry CONTENT_URI should return LocationEntry.CONTENT_TYPE",
		//                LocationEntry.CONTENT_TYPE, type);
		//    }


		/*
        This test uses the database directly to insert and then uses the ContentProvider to
        read out the data.  Uncomment this test to see if the basic weather Query functionality
        given in the ContentProvider is working correctly.
     */
		//    public void testBasicWeatherQuery() {
		//        // insert our test records into the database
		//        WeatherDbHelper dbHelper = new WeatherDbHelper(context);
		//        SQLiteDatabase db = dbHelper.getWritableDatabase();
		//
		//        ContentValues testValues = TestUtilities.createNorthPoleLocationValues();
		//        long locationRowId = TestUtilities.insertNorthPoleLocationValues(context);
		//
		//        // Fantastic.  Now that we have a location, add some weather!
		//        ContentValues weatherValues = TestUtilities.createWeatherValues(locationRowId);
		//
		//        long weatherRowId = db.insert(WeatherEntry.TABLE_NAME, null, weatherValues);
		//        assertTrue("Unable to Insert WeatherEntry into the Database", weatherRowId != -1);
		//
		//        db.close();
		//
		//        // Test the basic content provider Query
		//         weather = context.getContentResolver().Query(
		//                WeatherEntry.CONTENT_URI,
		//                null,
		//                null,
		//                null,
		//                null
		//        );
		//
		//        // Make sure we get the correct cursor out of the database
		//        TestUtilities.validate("testBasicWeatherQuery", weather, weatherValues);
		//    }

		/*
        This test uses the database directly to insert and then uses the ContentProvider to
        read out the data.  Uncomment this test to see if your location queries are
        performing correctly.
     */
		//    public void testBasicLocationQueries() {
		//        // insert our test records into the database
		//        WeatherDbHelper dbHelper = new WeatherDbHelper(context);
		//        SQLiteDatabase db = dbHelper.getWritableDatabase();
		//
		//        ContentValues testValues = TestUtilities.createNorthPoleLocationValues();
		//        long locationRowId = TestUtilities.insertNorthPoleLocationValues(context);
		//
		//        // Test the basic content provider Query
		//         location = context.getContentResolver().Query(
		//                LocationEntry.CONTENT_URI,
		//                null,
		//                null,
		//                null,
		//                null
		//        );
		//
		//        // Make sure we get the correct cursor out of the database
		//        TestUtilities.validate("testBasicLocationQueries, location Query", location, testValues);
		//
		//        // Has the NotificationUri been set correctly? --- we can only test this easily against API
		//        // level 19 or greater because getNotificationUri was added in API level 19.
		//        if ( Build.VERSION.SDK_INT >= 19 ) {
		//            Assert.AreEqual("Error: Location Query did not properly set NotificationUri",
		//                    location.getNotificationUri(), LocationEntry.CONTENT_URI);
		//        }
		//    }

		/*
        This test uses the provider to insert and then update the data. Uncomment this test to
        see if your update location is functioning correctly.
     */
		//    public void testUpdateLocation() {
		//        // Create a new map of values, where column names are the keys
		//        ContentValues values = TestUtilities.createNorthPoleLocationValues();
		//
		//        Uri locationUri = context.getContentResolver().
		//                insert(LocationEntry.CONTENT_URI, values);
		//        long locationRowId = ContentUris.parseId(locationUri);
		//
		//        // Verify we got a row back.
		//        assertTrue(locationRowId != -1);
		//        Log.d(LOG_TAG, "New row id: " + locationRowId);
		//
		//        ContentValues updatedValues = new ContentValues(values);
		//        updatedValues.put(LocationEntry._ID, locationRowId);
		//        updatedValues.put(LocationEntry.COLUMN_CITY_NAME, "Santa's Village");
		//
		//        // Create a cursor with observer to make sure that the content provider is notifying
		//        // the observers as expected
		//         location = context.getContentResolver().Query(LocationEntry.CONTENT_URI, null, null, null, null);
		//
		//        TestUtilities.TestContentObserver tco = TestUtilities.getTestContentObserver();
		//        location.registerContentObserver(tco);
		//
		//        int count = context.getContentResolver().update(
		//                LocationEntry.CONTENT_URI, updatedValues, LocationEntry._ID + "= ?",
		//                new String[] { Long.toString(locationRowId)});
		//        Assert.AreEqual(count, 1);
		//
		//        // Test to make sure our observer is called.  If not, we throw an assertion.
		//        //
		//        // Students: If your code is failing here, it means that your content provider
		//        // isn't calling getContext().getContentResolver().notifyChange(uri, null);
		//        tco.waitForNotificationOrFail();
		//
		//        location.unregisterContentObserver(tco);
		//        location.close();
		//
		//        // A cursor is your primary interface to the Query results.
		//         cursor = context.getContentResolver().Query(
		//                LocationEntry.CONTENT_URI,
		//                null,   // projection
		//                LocationEntry._ID + " = " + locationRowId,
		//                null,   // Values for the "where" clause
		//                null    // sort order
		//        );
		//
		//        TestUtilities.validate("testUpdateLocation.  Error validating location entry update.",
		//                cursor, updatedValues);
		//
		//        cursor.close();
		//    }


		// Make sure we can still Delete after adding/updating stuff
		//
		// Student: Uncomment this test after you have completed writing the insert functionality
		// in your provider.  It relies on insertions with testInsertReadProvider, so insert and
		// Query functionality must also be complete before this test can be used.
		//    public void testInsertReadProvider() {
		//        ContentValues testValues = TestUtilities.createNorthPoleLocationValues();
		//
		//        // Register a content observer for our insert.  This time, directly with the content resolver
		//        TestUtilities.TestContentObserver tco = TestUtilities.getTestContentObserver();
		//        context.getContentResolver().registerContentObserver(LocationEntry.CONTENT_URI, true, tco);
		//        Uri locationUri = context.getContentResolver().insert(LocationEntry.CONTENT_URI, testValues);
		//
		//        // Did our content observer get called?  Students:  If this fails, your insert location
		//        // isn't calling getContext().getContentResolver().notifyChange(uri, null);
		//        tco.waitForNotificationOrFail();
		//        context.getContentResolver().unregisterContentObserver(tco);
		//
		//        long locationRowId = ContentUris.parseId(locationUri);
		//
		//        // Verify we got a row back.
		//        assertTrue(locationRowId != -1);
		//
		//        // Data's inserted.  IN THEORY.  Now pull some out to stare at it and verify it made
		//        // the round trip.
		//
		//        // A cursor is your primary interface to the Query results.
		//         cursor = context.getContentResolver().Query(
		//                LocationEntry.CONTENT_URI,
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                null  // sort order
		//        );
		//
		//        TestUtilities.validate("testInsertReadProvider. Error validating LocationEntry.",
		//                cursor, testValues);
		//
		//        // Fantastic.  Now that we have a location, add some weather!
		//        ContentValues weatherValues = TestUtilities.createWeatherValues(locationRowId);
		//        // The TestContentObserver is a one-shot class
		//        tco = TestUtilities.getTestContentObserver();
		//
		//        context.getContentResolver().registerContentObserver(WeatherEntry.CONTENT_URI, true, tco);
		//
		//        Uri weatherInsertUri = context.getContentResolver()
		//                .insert(WeatherEntry.CONTENT_URI, weatherValues);
		//        assertTrue(weatherInsertUri != null);
		//
		//        // Did our content observer get called?  Students:  If this fails, your insert weather
		//        // in your ContentProvider isn't calling
		//        // getContext().getContentResolver().notifyChange(uri, null);
		//        tco.waitForNotificationOrFail();
		//        context.getContentResolver().unregisterContentObserver(tco);
		//
		//        // A cursor is your primary interface to the Query results.
		//         weather = context.getContentResolver().Query(
		//                WeatherEntry.CONTENT_URI,  // Table to Query
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                null // columns to group by
		//        );
		//
		//        TestUtilities.validate("testInsertReadProvider. Error validating WeatherEntry insert.",
		//                weather, weatherValues);
		//
		//        // Add the location values in with the weather data so that we can make
		//        // sure that the join worked and we actually get all the values back
		//        weatherValues.putAll(testValues);
		//
		//        // Get the joined Weather and Location data
		//        weather = context.getContentResolver().Query(
		//                WeatherEntry.buildWeatherLocation(TestUtilities.TEST_LOCATION),
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                null  // sort order
		//        );
		//        TestUtilities.validate("testInsertReadProvider.  Error validating joined Weather and Location Data.",
		//                weather, weatherValues);
		//
		//        // Get the joined Weather and Location data with a start date
		//        weather = context.getContentResolver().Query(
		//                WeatherEntry.buildWeatherLocationWithStartDate(
		//                        TestUtilities.TEST_LOCATION, TestUtilities.TEST_DATE),
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                null  // sort order
		//        );
		//        TestUtilities.validate("testInsertReadProvider.  Error validating joined Weather and Location Data with start date.",
		//                weather, weatherValues);
		//
		//        // Get the joined Weather data for a specific date
		//        weather = context.getContentResolver().Query(
		//                WeatherEntry.buildWeatherLocationWithDate(TestUtilities.TEST_LOCATION, TestUtilities.TEST_DATE),
		//                null,
		//                null,
		//                null,
		//                null
		//        );
		//        TestUtilities.validate("testInsertReadProvider.  Error validating joined Weather and Location data for a specific date.",
		//                weather, weatherValues);
		//    }

		// Make sure we can still Delete after adding/updating stuff
		//
		// Student: Uncomment this test after you have completed writing the Delete functionality
		// in your provider.  It relies on insertions with testInsertReadProvider, so insert and
		// Query functionality must also be complete before this test can be used.
		//    public void testDeleteRecords() {
		//        testInsertReadProvider();
		//
		//        // Register a content observer for our location Delete.
		//        TestUtilities.TestContentObserver locationObserver = TestUtilities.getTestContentObserver();
		//        context.getContentResolver().registerContentObserver(LocationEntry.CONTENT_URI, true, locationObserver);
		//
		//        // Register a content observer for our weather Delete.
		//        TestUtilities.TestContentObserver weatherObserver = TestUtilities.getTestContentObserver();
		//        context.getContentResolver().registerContentObserver(WeatherEntry.CONTENT_URI, true, weatherObserver);
		//
		//        DeleteAllRecordsFromProvider();
		//
		//        // Students: If either of these fail, you most-likely are not calling the
		//        // getContext().getContentResolver().notifyChange(uri, null); in the ContentProvider
		//        // Delete.  (only if the insertReadProvider is succeeding)
		//        locationObserver.waitForNotificationOrFail();
		//        weatherObserver.waitForNotificationOrFail();
		//
		//        context.getContentResolver().unregisterContentObserver(locationObserver);
		//        context.getContentResolver().unregisterContentObserver(weatherObserver);
		//    }


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
				weatherValues.Put (WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_ID, 321);
				returnContentValues [i] = weatherValues;
			}
			return returnContentValues;
		}

		// Student: Uncomment this test after you have completed writing the BulkInsert functionality
		// in your provider.  Note that this test will work with the built-in (default) provider
		// implementation, which just inserts records one-at-a-time, so really do implement the
		// BulkInsert ContentProvider function.
		//    public void testBulkInsert() {
		//        // first, let's create a location value
		//        ContentValues testValues = TestUtilities.createNorthPoleLocationValues();
		//        Uri locationUri = context.getContentResolver().insert(LocationEntry.CONTENT_URI, testValues);
		//        long locationRowId = ContentUris.parseId(locationUri);
		//
		//        // Verify we got a row back.
		//        assertTrue(locationRowId != -1);
		//
		//        // Data's inserted.  IN THEORY.  Now pull some out to stare at it and verify it made
		//        // the round trip.
		//
		//        // A cursor is your primary interface to the Query results.
		//         cursor = context.getContentResolver().Query(
		//                LocationEntry.CONTENT_URI,
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                null  // sort order
		//        );
		//
		//        TestUtilities.validate("testBulkInsert. Error validating LocationEntry.",
		//                cursor, testValues);
		//
		//        // Now we can bulkInsert some weather.  In fact, we only implement BulkInsert for weather
		//        // entries.  With ContentProviders, you really only have to implement the features you
		//        // use, after all.
		//        ContentValues[] bulkInsertContentValues = createBulkInsertWeatherValues(locationRowId);
		//
		//        // Register a content observer for our bulk insert.
		//        TestUtilities.TestContentObserver weatherObserver = TestUtilities.getTestContentObserver();
		//        context.getContentResolver().registerContentObserver(WeatherEntry.CONTENT_URI, true, weatherObserver);
		//
		//        int insertCount = context.getContentResolver().bulkInsert(WeatherEntry.CONTENT_URI, bulkInsertContentValues);
		//
		//        // Students:  If this fails, it means that you most-likely are not calling the
		//        // getContext().getContentResolver().notifyChange(uri, null); in your BulkInsert
		//        // ContentProvider method.
		//        weatherObserver.waitForNotificationOrFail();
		//        context.getContentResolver().unregisterContentObserver(weatherObserver);
		//
		//        Assert.AreEqual(insertCount, BULK_INSERT_RECORDS_TO_INSERT);
		//
		//        // A cursor is your primary interface to the Query results.
		//        cursor = context.getContentResolver().Query(
		//                WeatherEntry.CONTENT_URI,
		//                null, // leaving "columns" null just returns all the columns.
		//                null, // cols for "where" clause
		//                null, // values for "where" clause
		//                WeatherEntry.COLUMN_DATE + " ASC"  // sort order == by DATE ASCENDING
		//        );
		//
		//        // we should have as many records in the database as we've inserted
		//        Assert.AreEqual(cursor.getCount(), BULK_INSERT_RECORDS_TO_INSERT);
		//
		//        // and let's make sure they match the ones we created
		//        cursor.moveToFirst();
		//        for ( int i = 0; i < BULK_INSERT_RECORDS_TO_INSERT; i++, cursor.moveToNext() ) {
		//            TestUtilities.validateCurrentRecord("testBulkInsert.  Error validating WeatherEntry " + i,
		//                    cursor, bulkInsertContentValues[i]);
		//        }
		//        cursor.close();
		//    }
	}
}

