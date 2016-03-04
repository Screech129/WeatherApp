using System;
using Android.Content;
using WeatherApp;
using NUnit.Framework;
using Android.Database;

namespace AndroidTest
{
	[TestFixture]
	public class TestFetchWeatherTask
	{
		public TestFetchWeatherTask ()
		{
		}

		Context context = Android.App.Application.Context;

		const string ADD_LOCATION_SETTING = "Sunnydale, CA";
		const string ADD_LOCATION_CITY = "Sunnydale";
		const double ADD_LOCATION_LAT = 34.425833;
		const double ADD_LOCATION_LON = -119.714167;

		/*
        Students: uncomment testAddLocation after you have written the AddLocation function.
        This test will only run on API level 11 and higher because of a requirement in the
        content provider.
     */
		//    @TargetApi(11)
		[Test]
		public void testAddLocation ()
		{
			// start from a clean state
			context.ContentResolver.Delete (WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ?",
				new String[]{ ADD_LOCATION_SETTING });
		
			FetchWeatherTask fwt = new FetchWeatherTask (context);
			long locationId = fwt.AddLocation (ADD_LOCATION_SETTING, ADD_LOCATION_CITY,
				                  ADD_LOCATION_LAT, ADD_LOCATION_LON);
		
			// does addLocation return a valid record ID?
			Assert.IsFalse (locationId == -1, "Error: addLocation returned an invalid ID on insert");
		
			// test all this twice
			for (int i = 0; i < 2; i++) {
		
				// does the ID point to our location?
				ICursor locationCursor = context.ContentResolver.Query (
					                         WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
					                         new String[] {
						WeatherContractOpen.LocationEntryOpen._ID,
						WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING,
						WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME,
						WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT,
						WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG
					},
					                         WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ?",
					                         new String[]{ ADD_LOCATION_SETTING },
					                         null);
		
				// these match the indices of the projection
				if (locationCursor.MoveToFirst ()) {
					Assert.AreEqual (locationCursor.GetLong (0), locationId, "Error: the queried value of locationId does not match the returned value" +
					"from addLocation");
					Assert.AreEqual (locationCursor.GetString (1), ADD_LOCATION_SETTING, "Error: the queried value of location setting is incorrect");
					Assert.AreEqual (locationCursor.GetString (2), ADD_LOCATION_CITY, "Error: the queried value of location city is incorrect");
					Assert.AreEqual (locationCursor.GetDouble (3), ADD_LOCATION_LAT, "Error: the queried value of latitude is incorrect");
					Assert.AreEqual (locationCursor.GetDouble (4), ADD_LOCATION_LON, "Error: the queried value of longitude is incorrect");
				} else {
					throw new Exception ("Error: the id you used to query returned an empty cursor");
				}
		
				// there should be no more records
				Assert.IsFalse (locationCursor.MoveToNext (), "Error: there should be only one record returned from a location query");
		
				// add the location again
				long newLocationId = fwt.AddLocation (ADD_LOCATION_SETTING, ADD_LOCATION_CITY,
					                     ADD_LOCATION_LAT, ADD_LOCATION_LON);
		
				Assert.AreEqual (locationId, newLocationId, "Error: inserting a location again should return the same ID");
			}
			// reset our state back to normal
			context.ContentResolver.Delete (WeatherContractOpen.LocationEntryOpen.CONTENT_URI,
				WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ?",
				new String[]{ ADD_LOCATION_SETTING });
		
			// clean up the test so that other tests can use the content provider
			context.ContentResolver.
		                AcquireContentProviderClient (WeatherContractOpen.LocationEntryOpen.CONTENT_URI).
		                LocalContentProvider.Shutdown ();
		}
	}
}

