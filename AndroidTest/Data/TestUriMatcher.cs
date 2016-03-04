using System;
using WeatherApp;
using Android.Content;
using NUnit.Framework;

namespace AndroidTest
{
	[TestFixture]
	public class TestUriMatcher
	{
		public TestUriMatcher ()
		{
		}

		private const string LOCATION_QUERY = "London, UK";
		private const long TEST_DATE = 1419033600L;
		// December 20th, 2014
		private const long TEST_LOCATION_id = 10L;

		// content://com.example.android.sunshine.app/weather"
		private static  Android.Net.Uri TEST_WEATHER_DIR = WeatherContractOpen.WeatherEntryOpen.CONTENT_URI;
		private static  Android.Net.Uri TEST_WEATHER_WITH_LOCATION_DIR = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (LOCATION_QUERY);
		private static  Android.Net.Uri TEST_WEATHER_WITH_LOCATION_AND_DATE_DIR = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (LOCATION_QUERY, TEST_DATE);
		// content://com.example.android.sunshine.app/location"
		private static Android.Net.Uri TEST_LOCATION_DIR = WeatherContractOpen.LocationEntryOpen.CONTENT_URI;

		/*
        Students: This function tests that your UriMatcher returns the correct integer value
        for each of the Uri types that our ContentProvider can handle.  Uncomment this when you are
        ready to test your UriMatcher.
     */
		[Test]
		public void testUriMatcher ()
		{
			UriMatcher testMatcher = WeatherProvider.buildUriMatcher ();
		
			Assert.AreEqual (testMatcher.Match (TEST_WEATHER_DIR), WeatherProvider.WEATHER, "Error: The WEATHER URI was matched incorrectly.");
			Assert.AreEqual (testMatcher.Match (TEST_WEATHER_WITH_LOCATION_DIR), WeatherProvider.WEATHER_WITH_LOCATION, "Error: The WEATHER WITH LOCATION URI was matched incorrectly.");
			Assert.AreEqual (testMatcher.Match (TEST_WEATHER_WITH_LOCATION_AND_DATE_DIR), WeatherProvider.WEATHER_WITH_LOCATION_AND_DATE, "Error: The WEATHER WITH LOCATION AND DATE URI was matched incorrectly.");
			Assert.AreEqual (testMatcher.Match (TEST_LOCATION_DIR), WeatherProvider.LOCATION, "Error: The LOCATION URI was matched incorrectly.");
		}
	}
}

