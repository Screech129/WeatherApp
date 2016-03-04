using System;
using Android.Content;
using Android.Preferences;
using Android.Content.Res;

namespace WeatherApp
{
	public class Utility
	{
		public Utility ()
		{
		}

		public static string getPreferredLocation (Context context)
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (context);
			var zip = prefs.GetString (context.GetString (Resource.String.pref_location_key), context.GetString (Resource.String.pref_location_default));
			return zip;
		}

		public static bool isMetric (Context context)
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (context);
			return prefs.GetString (context.GetString (Resource.String.pref_temp_key), context.GetString (Resource.String.pref_temp_default))
					.Equals (context.GetString (Resource.String.pref_units_metric));
		}

		public static String formatTemperature (double temperature, bool isMetric)
		{
			double temp;
			if (!isMetric) {
				temp = 9 * temperature / 5 + 32;
			} else {
				temp = temperature;
			}
			return String.Format ("{0:0}.0f", temp);
		}

		public static String formatDate (long dateInMillis)
		{
			DateTime date = new DateTime (dateInMillis);
			return date.ToString ("ddd, MMM dd");
		}
	}
}

