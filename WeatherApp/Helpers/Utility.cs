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

		public const string DATE_FORMAT = "yyyyMMdd";

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

		public static String formatTemperature (Context context, double temperature, bool isMetric)
		{
			double temp;
			if (!isMetric) {
				temp = 9 * temperature / 5 + 32;
			} else {
				temp = temperature;
			}
			return String.Format (context.GetString (Resource.String.format_temperature), temp);
		}

		public static String formatDate (long dateInMillis)
		{
			DateTime date = new DateTime (dateInMillis);
			return date.ToString ("ddd, MMM dd");
		}

		public static string getFriendlyDayString (Context context, long dateInMillis)
		{
			// The day string for forecast uses the following logic:
			// For today: "Today, June 8"
			// For tomorrow:  "Tomorrow"
			// For the next 5 days: "Wednesday" (just the day name)
			// For all days after that: "Mon Jun 8"

			var todayDate = DateTime.Now.ToLocalTime ().Date;

			long currentDay = DateTime.Today.Ticks;
			DateTime dateAsDay = new DateTime ();
			var forecastDay = dateAsDay.AddTicks (dateInMillis).ToLocalTime ().Date;

			// If the date we're building the String for is today's date, the format
			// is "Today, June 24"
			if (todayDate == forecastDay) {
				String today = context.GetString (Resource.String.today);
				int formatId = Resource.String.format_full_friendly_date;
				return String.Format (context.GetString (
					formatId),
					today,
					getFormattedMonthDay (context, dateInMillis));
			} else if (currentDay < dateInMillis + 7) {
				// If the input date is less than a week in the future, just return the day name.
				return getDayName (context, dateInMillis);
			} else {
				// Otherwise, use the form "Mon Jun 3"
				return dateInMillis.ToString ("ddd MMM dd");
			}
		}

		/**
     * Given a day, returns just the name to use for that day.
     * E.g "today", "tomorrow", "wednesday".
     *
     * @param context Context to use for resource localization
     * @param dateInMillis The date in milliseconds
     * @return
     */
		public static String getDayName (Context context, long dateInMillis)
		{
			// If the date is today, return the localized version of "Today" instead of the actual
			// day name.

			var todayDate = DateTime.Now.ToLocalTime ().Date;

			long currentDay = DateTime.Today.Ticks;
			DateTime dateAsDay = new DateTime ();
			var forecastDay = dateAsDay.AddTicks (dateInMillis).ToLocalTime ().Date;

			if (todayDate == forecastDay) {
				return context.GetString (Resource.String.today);
			} else if (forecastDay == todayDate.AddDays (1)) {
				return context.GetString (Resource.String.tomorrow);
			} else {
				
				// Otherwise, the format is just the day of the week (e.g "Wednesday".
				return forecastDay.ToString ("dddd");
			}
		}

		/**
     * Converts db date format to the format "Month day", e.g "June 24".
     * @param context Context to use for resource localization
     * @param dateInMillis The db formatted date string, expected to be of the form specified
     *                in Utility.DATE_FORMAT
     * @return The day in the form of a string formatted "December 6"
     */
		public static String getFormattedMonthDay (Context context, long dateInMillis)
		{
			DateTime date = new DateTime (dateInMillis);
			return date.ToString ("MMMM dd");
		}

		public static String getFormattedWind (Context context, float windSpeed, float degrees)
		{
			int windFormat;
			if (Utility.isMetric (context)) {
				windFormat = Resource.String.format_wind_kmh;
			} else {
				windFormat = Resource.String.format_wind_mph;
				windSpeed = .621371192237334f * windSpeed;
			}

			// From wind direction in degrees, determine compass direction as a string (e.g NW)
			// You know what's fun, writing really long if/else statements with tons of possible
			// conditions.  Seriously, try it!
			String direction = "Unknown";
			if (degrees >= 337.5 || degrees < 22.5) {
				direction = "N";
			} else if (degrees >= 22.5 && degrees < 67.5) {
				direction = "NE";
			} else if (degrees >= 67.5 && degrees < 112.5) {
				direction = "E";
			} else if (degrees >= 112.5 && degrees < 157.5) {
				direction = "SE";
			} else if (degrees >= 157.5 && degrees < 202.5) {
				direction = "S";
			} else if (degrees >= 202.5 && degrees < 247.5) {
				direction = "SW";
			} else if (degrees >= 247.5 && degrees < 292.5) {
				direction = "W";
			} else if (degrees >= 292.5 || degrees < 22.5) {
				direction = "NW";
			}
			return String.Format (context.GetString (windFormat), windSpeed, direction);
		}

        public static int getIconResourceForWeatherCondition (int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            if (weatherId >= 200 && weatherId <= 232)
            {
                return Resource.Drawable.ic_storm;
            }
            else if (weatherId >= 300 && weatherId <= 321)
            {
                return Resource.Drawable.ic_light_rain;
            }
            else if (weatherId >= 500 && weatherId <= 504)
            {
                return Resource.Drawable.ic_rain;
            }
            else if (weatherId == 511)
            {
                return Resource.Drawable.ic_snow;
            }
            else if (weatherId >= 520 && weatherId <= 531)
            {
                return Resource.Drawable.ic_rain;
            }
            else if (weatherId >= 600 && weatherId <= 622)
            {
                return Resource.Drawable.ic_snow;
            }
            else if (weatherId >= 701 && weatherId <= 761)
            {
                return Resource.Drawable.ic_fog;
            }
            else if (weatherId == 761 || weatherId == 781)
            {
                return Resource.Drawable.ic_storm;
            }
            else if (weatherId == 800)
            {
                return Resource.Drawable.ic_clear;
            }
            else if (weatherId == 801)
            {
                return Resource.Drawable.ic_light_clouds;
            }
            else if (weatherId >= 802 && weatherId <= 804)
            {
                return Resource.Drawable.ic_cloudy;
            }
            return -1;
        }

    }
}

