using System;
using NUnit.Framework;
using Android.Content;
using WeatherApp;
using System.Collections.Generic;
using Android.Database.Sqlite;
using Android.Database;

namespace AndroidTest
{
	[TestFixture]
	public class TestDbOpen
	{
		public const string LOG_TAG = "TestDbOpen";
		Context context = Android.App.Application.Context;
		ICursor c;

		void deleteTheDatabase ()
		{
			context.DeleteDatabase (WeatherDbOpenHelper.DATABASE_NAME);
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
			HashSet<String> tableNameHashSet = new HashSet<String> ();
			tableNameHashSet.Add (WeatherContractOpen.LocationEntryOpen.TABLE_NAME);
			tableNameHashSet.Add (WeatherContractOpen.WeatherEntryOpen.TABLE_NAME);
			
			context.DeleteDatabase (WeatherDbOpenHelper.DATABASE_NAME);
			SQLiteDatabase db = new WeatherDbOpenHelper (
				                    this.context).WritableDatabase;
			Assert.AreEqual (true, db.IsOpen);
			
			// have we created the tables we want?
			c = db.RawQuery ("SELECT name FROM sqlite_master WHERE type='table'", null);

			Assert.IsTrue (c.MoveToFirst (), "Error: This means that the database has not been created correctly");
			
			// verify that the tables have been created
			do {
				tableNameHashSet.Remove (c.GetString (0));
			} while(c.MoveToNext ());
			
			// if this fails, it means that your database doesn't contain both the location entry
			// and weather entry tables
			Assert.IsTrue (tableNameHashSet.Count == 0, "Error: Your database was created without both the location entry and weather entry tables");
			
			// now, do our tables contain the correct columns?
			c = db.RawQuery ("PRAGMA table_info(" + WeatherContractOpen.LocationEntryOpen.TABLE_NAME + ")",
				null);
			
			Assert.IsTrue (c.MoveToFirst (), "Error: This means that we were unable to query the database for table information.");

			// Build a HashSet of all of the column names we want to look for
			HashSet<String> locationColumnHashSet = new HashSet<String> ();
			locationColumnHashSet.Add (WeatherContractOpen.LocationEntryOpen._ID);
			locationColumnHashSet.Add (WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME);
			locationColumnHashSet.Add (WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT);
			locationColumnHashSet.Add (WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG);
			locationColumnHashSet.Add (WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING);
			
			int columnNameIndex = c.GetColumnIndex ("name");
			do {
				String columnName = c.GetString (columnNameIndex);
				locationColumnHashSet.Remove (columnName);
			} while(c.MoveToNext ());
			
			// if this fails, it means that your database doesn't contain all of the required location
			// entry columns
			Assert.IsTrue (locationColumnHashSet.Count == 0, "Error: The database doesn't contain all of the required location entry columns");
			db.Close ();
		}

		[Test]
		public void testLocationTable ()
		{
			// First step: Get reference to writable database
			var dbHelper = new WeatherDbOpenHelper (context);
			var db = dbHelper.WritableDatabase;
			// Create ContentValues of what you want to insert
			// (you can use the createNorthPoleLocationValues if you wish)
			var expectedValues = TestUtilitiesOpen.createNorthPoleLocationValues ();
			// Insert ContentValues into database and get a row ID back
			var rowId = TestUtilitiesOpen.insertNorthPoleLocationValues (context);
			Assert.IsTrue (rowId > 0, "Row was not inserted.");
			// Query the database and receive a Cursor back
			var rowInfo = db.Query (WeatherContractOpen.LocationEntryOpen.TABLE_NAME, null, null, null, null, null, null);
			// Move the cursor to a valid database row
			Assert.IsTrue (rowInfo.MoveToFirst (), "No data found.");
			// Validate data in resulting Cursor with the original ContentValues
			// (you can use the validateCurrentRecord function in TestUtilities to validate the
			// query if you like)
			TestUtilitiesOpen.validateCurrentRecord ("Record is not valid", rowInfo, expectedValues);

			//Ensure only one record exist
			Assert.IsFalse (rowInfo.MoveToNext (), "More than one record was returned.");
			// Finally, close the cursor and database
			rowInfo.Close ();
			db.Close ();


		}

		[Test]
		public void testWeatherTable ()
		{
			// First insert the location, and then use the locationRowId to insert
			// the weather. Make sure to cover as many failure cases as you can.

			// Instead of rewriting all of the code we've already written in testLocationTable
			// we can move this code to insertLocation and then call insertLocation from both
			// tests. Why move it? We need the code to return the ID of the inserted location
			// and our testLocationTable can only return void because it's a test.

			// First step: Get reference to writable database
			var dbHelper = new WeatherDbOpenHelper (context);
			var db = dbHelper.WritableDatabase;

			// Create ContentValues of what you want to insert
			// (you can use the createWeatherValues TestUtilities function if you wish)

			// Insert ContentValues into database and get a row ID back
			TestUtilitiesOpen.insertWeatherValues (context);

			// Query the database and receive a Cursor back
			var rowInfo = db.Query (WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, null, null, null, null, null, null);
			// Move the cursor to a valid database row
			Assert.IsTrue (rowInfo.MoveToFirst (), "No rows were inserted.");
			// Validate data in resulting Cursor with the original ContentValues
			// (you can use the validateCurrentRecord function in TestUtilities to validate the
			// query if you like)
			TestUtilitiesOpen.validateCurrentRecord ("Weather values not valid", rowInfo, TestUtilitiesOpen.createWeatherValues (1));

			Assert.IsFalse (rowInfo.MoveToNext (), "Error: More than one record was inserted.");
			// Finally, close the cursor and database
			rowInfo.Close ();
			db.Close ();
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

