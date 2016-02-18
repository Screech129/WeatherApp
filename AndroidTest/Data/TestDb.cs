using System;
using NUnit.Framework;
using System.Collections.Generic;
using WeatherApp;
using SQLite;
using System.IO;
using System.Linq;
using System.Data;
using AndroidTest;

namespace AndroidTest
{
	[TestFixture]
	public class TestDb
	{
		const String DATABASE_NAME = "weather.db";
		static string personalFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		static readonly string DATABASE_PATH = Path.Combine (personalFolder, DATABASE_NAME);
		public const string LOG_TAG = "TestDb";
		WeatherDbHelper helper = new WeatherDbHelper ();
		SQLiteConnection con = new SQLiteConnection (DATABASE_PATH);

		void deleteTheDatabase ()
		{
			helper.DropAll ();
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
			using (SQLiteConnection conn = new SQLiteConnection (DATABASE_PATH)) {
				TableMapping map = new TableMapping (typeof(SqlDbType));
				helper.Create ();
				var tables = helper.Query (map, "SELECT name FROM sqlite_master WHERE type='table'", new object[0]);


				// have we created the tables we want?

				Assert.IsTrue (tables.Count >= 2, "Error: This means that the database has not been created correctly");


				// now, do our tables contain the correct columns?
				var columnNames = helper.GetTableInfo ("location");

				Assert.IsTrue (columnNames.Count > 0, "Error: This means that the database has not been created correctly");

				// Build a HashSet of all of the column names we want to look for
				HashSet<String> locationColumnHashSet = new HashSet<String> ();
				locationColumnHashSet.Add ("Count");
				locationColumnHashSet.Add ("city_name");
				locationColumnHashSet.Add ("coord_lat");
				locationColumnHashSet.Add ("coord_long");
				locationColumnHashSet.Add ("location_setting");
				var columnList = columnNames.ToList ();
				var columnNameIndex = 0;
				do {
					var columnName = columnList [columnNameIndex].Name;
					locationColumnHashSet.Remove (columnName);
					columnNameIndex++;
				} while(columnNameIndex < columnList.Count);

				// if this fails, it means that your database doesn't contain all of the required location
				// entry columns
				Assert.IsTrue (locationColumnHashSet.Count == 0, "Error: The database doesn't contain all of the required location entry columns");
			}

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
			using (SQLiteConnection conn = new SQLiteConnection (DATABASE_PATH)) {
				
				helper.Create ();

				var result = TestUtilities.insertNorthPoleLocationValues ();
				Assert.IsTrue (result > 0, "Error: No rows were inserted");
				var test = helper.Table<LocationEntry> ();
				var resultSet = helper.Get<LocationEntry> (result);
				var resultAssert = TestUtilities.validateCurrentRecord (resultSet);
				Assert.IsTrue (resultAssert, "Error during validation");
			}

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
			using (SQLiteConnection conn = new SQLiteConnection (DATABASE_PATH)) {
				helper.Create ();
				TestUtilities.insertNorthPoleLocationValues ();
				var locValues = helper.Table<LocationEntry> ();
				var result = TestUtilities.insertFakeWeather ();
				Assert.IsTrue (result > 0, "Error: No rows were inserted");
				var test = helper.Table<WeatherEntry> ();
				var resultSet = helper.Get<WeatherEntry> (result);
				var resultAssert = TestUtilities.validateCurrentRecordWeath (resultSet);
				Assert.IsTrue (resultAssert, "Error during validation");
			}
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

