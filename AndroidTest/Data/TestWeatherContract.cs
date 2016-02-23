using System;
using WeatherApp;
using NUnit.Framework;

namespace AndroidTest
{
	[TestFixture]
	public class TestWeatherContract
	{
		public TestWeatherContract ()
		{
		}

		// intentionally includes a slash to make sure Uri is getting quoted correctly
		private const string TEST_WEATHER_LOCATION = "/North Pole";
		private const long TEST_WEATHER_DATE = 1419033600L;
		// December 20th, 2014

		/*
        Students: Uncomment this out to test your weather location function.
     */
		[Test]
		public void testBuildWeatherLocation ()
		{
			Android.Net.Uri locationUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocation (TEST_WEATHER_LOCATION);
			Assert.NotNull (locationUri, "Error: Null Uri returned.  You must fill-in buildWeatherLocation in " +
			"WeatherContract.");
			Assert.AreEqual (TEST_WEATHER_LOCATION, locationUri.LastPathSegment, "Error: Weather location not properly appended to the end of the Uri");
			Assert.AreEqual (locationUri.ToString (),
				"content://WeatherApp/weather/%2FNorth%20Pole", "Error: Weather location Uri doesn't match our expected result");
		                
		}
	}
}

