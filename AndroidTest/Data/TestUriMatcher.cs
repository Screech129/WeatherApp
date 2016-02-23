using System;
using WeatherApp;

namespace AndroidTest
{
	public class TestUriMatcher
	{
		public TestUriMatcher ()
		{
		}

		private const string LOCATION_QUERY = "London, UK";
		private const long TEST_DATE = 1419033600L;
		// December 20th, 2014
		private const long TEST_LOCATION_ID = 10L;

		// content://com.example.android.sunshine.app/weather"
		private  Android.Net.Uri TEST_WEATHER_DIR = WeatherContractOpen.WeatherEntryOpen.CONTENT_URI;
		private  Android.Net.Uri TEST_WEATHER_WITH_LOCATION_DIR = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (LOCATION_QUERY);
		private  Android.Net.Uri TEST_WEATHER_WITH_LOCATION_AND_DATE_DIR = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate (LOCATION_QUERY, TEST_DATE);
		// content://com.example.android.sunshine.app/location"
		private  Android.Net.Uri TEST_LOCATION_DIR = WeatherContractOpen.LocationEntryOpen.CONTENT_URI;

		/*
        Students: This function tests that your UriMatcher returns the correct integer value
        for each of the Uri types that our ContentProvider can handle.  Uncomment this when you are
        ready to test your UriMatcher.
     */
		//    public void testUriMatcher() {
		//        UriMatcher testMatcher = WeatherProvider.buildUriMatcher();
		//
		//        assertEquals("Error: The WEATHER URI was matched incorrectly.",
		//                testMatcher.match(TEST_WEATHER_DIR), WeatherProvider.WEATHER);
		//        assertEquals("Error: The WEATHER WITH LOCATION URI was matched incorrectly.",
		//                testMatcher.match(TEST_WEATHER_WITH_LOCATION_DIR), WeatherProvider.WEATHER_WITH_LOCATION);
		//        assertEquals("Error: The WEATHER WITH LOCATION AND DATE URI was matched incorrectly.",
		//                testMatcher.match(TEST_WEATHER_WITH_LOCATION_AND_DATE_DIR), WeatherProvider.WEATHER_WITH_LOCATION_AND_DATE);
		//        assertEquals("Error: The LOCATION URI was matched incorrectly.",
		//                testMatcher.match(TEST_LOCATION_DIR), WeatherProvider.LOCATION);
		//    }
	}
}

