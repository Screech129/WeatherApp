﻿using System;
using NUnit.Framework;

namespace AndroidTest
{
	[TestFixture]
	public class TestDb
	{
        
		public const string LOG_TAG = "TestDb";

		void deleteTheDatabase ()
		{
			//TestContext.deleteDatabase(WeatherDbHelper.DATABASE_NAME);
		}

		[SetUp]
		public void Setup ()
		{
			deleteTheDatabase ();
		}


		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void Create_Database ()
		{
			//        final HashSet<String> tableNameHashSet = new HashSet<String>();
			//        tableNameHashSet.add(WeatherContract.LocationEntry.TABLE_NAME);
			//        tableNameHashSet.add(WeatherContract.WeatherEntry.TABLE_NAME);
			//
			//        mContext.deleteDatabase(WeatherDbHelper.DATABASE_NAME);
			//        SQLiteDatabase db = new WeatherDbHelper(
			//                this.mContext).getWritableDatabase();
			//        assertEquals(true, db.isOpen());
			//
			//        // have we created the tables we want?
			//        Cursor c = db.rawQuery("SELECT name FROM sqlite_master WHERE type='table'", null);
			//
			//        assertTrue("Error: This means that the database has not been created correctly",
			//                c.moveToFirst());
			//
			//        // verify that the tables have been created
			//        do {
			//            tableNameHashSet.remove(c.getString(0));
			//        } while( c.moveToNext() );
			//
			//        // if this fails, it means that your database doesn't contain both the location entry
			//        // and weather entry tables
			//        assertTrue("Error: Your database was created without both the location entry and weather entry tables",
			//                tableNameHashSet.isEmpty());
			//
			//        // now, do our tables contain the correct columns?
			//        c = db.rawQuery("PRAGMA table_info(" + WeatherContract.LocationEntry.TABLE_NAME + ")",
			//                null);
			//
			//        assertTrue("Error: This means that we were unable to query the database for table information.",
			//                c.moveToFirst());
			//
			//        // Build a HashSet of all of the column names we want to look for
			//        final HashSet<String> locationColumnHashSet = new HashSet<String>();
			//        locationColumnHashSet.add(WeatherContract.LocationEntry._ID);
			//        locationColumnHashSet.add(WeatherContract.LocationEntry.COLUMN_CITY_NAME);
			//        locationColumnHashSet.add(WeatherContract.LocationEntry.COLUMN_COORD_LAT);
			//        locationColumnHashSet.add(WeatherContract.LocationEntry.COLUMN_COORD_LONG);
			//        locationColumnHashSet.add(WeatherContract.LocationEntry.COLUMN_LOCATION_SETTING);
			//
			//        int columnNameIndex = c.getColumnIndex("name");
			//        do {
			//            String columnName = c.getString(columnNameIndex);
			//            locationColumnHashSet.remove(columnName);
			//        } while(c.moveToNext());
			//
			//        // if this fails, it means that your database doesn't contain all of the required location
			//        // entry columns
			//        assertTrue("Error: The database doesn't contain all of the required location entry columns",
			//                locationColumnHashSet.isEmpty());
			//        db.close();
		}

		/*
        Students:  Here is where you will build code to test that we can insert and query the
        location database.  We've done a lot of work for you.  You'll want to look in TestUtilities
        where you can uncomment out the "createNorthPoleLocationValues" function.  You can
        also make use of the ValidateCurrentRecord function from within TestUtilities.
    */
		[Test]
		public void Insert_and_Query_Location_Database ()
		{
			// First step: Get reference to writable database

			// Create ContentValues of what you want to insert
			// (you can use the createNorthPoleLocationValues if you wish)

			// Insert ContentValues into database and get a row ID back

			// Query the database and receive a Cursor back

			// Move the cursor to a valid database row

			// Validate data in resulting Cursor with the original ContentValues
			// (you can use the validateCurrentRecord function in TestUtilities to validate the
			// query if you like)

			// Finally, close the cursor and database

		}

		/*
        Students:  Here is where you will build code to test that we can insert and query the
        database.  We've done a lot of work for you.  You'll want to look in TestUtilities
        where you can use the "createWeatherValues" function.  You can
        also make use of the validateCurrentRecord function from within TestUtilities.
     */
		[Test]
		public void Insert_and_Query_The_Database ()
		{
			// First insert the location, and then use the locationRowId to insert
			// the weather. Make sure to cover as many failure cases as you can.

			// Instead of rewriting all of the code we've already written in testLocationTable
			// we can move this code to insertLocation and then call insertLocation from both
			// tests. Why move it? We need the code to return the ID of the inserted location
			// and our testLocationTable can only return void because it's a test.

			// First step: Get reference to writable database

			// Create ContentValues of what you want to insert
			// (you can use the createWeatherValues TestUtilities function if you wish)

			// Insert ContentValues into database and get a row ID back

			// Query the database and receive a Cursor back

			// Move the cursor to a valid database row

			// Validate data in resulting Cursor with the original ContentValues
			// (you can use the validateCurrentRecord function in TestUtilities to validate the
			// query if you like)

			// Finally, close the cursor and database
		}


		/*
        Students: This is a helper method for the testWeatherTable quiz. You can move your
        code from testLocationTable to here so that you can call this code from both
        testWeatherTable and testLocationTable.
     */
		public long insertLocation ()
		{
			return -1L;
		}
	}
    

}
