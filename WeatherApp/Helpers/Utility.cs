using System;
using Android.Content;
using Android.Net;
using Android.Preferences;
using WeatherApp.Helpers;
using Object = Java.Lang.Object;

namespace WeatherApp
{
    public class Utility
    {
        public const string DATE_FORMAT = "yyyyMMdd";

        public static string GetPreferredLocation (Context context)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var zip = prefs.GetString(context.GetString(Resource.String.pref_location_key),
                context.GetString(Resource.String.pref_location_default));
            return zip;
        }

        public static bool IsMetric(Context context)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return prefs.GetString(context.GetString(Resource.String.pref_temp_key),
                context.GetString(Resource.String.pref_temp_default))
                .Equals(context.GetString(Resource.String.pref_units_metric));
        }

        public static string FormatTemperature (Context context, double temperature, bool isMetric)
        {
            double temp;
            if (!isMetric)
            {
                temp = (temperature * 1.8) + 32;
            }
            else
            {
                temp = temperature;
            }
            return string.Format(context.GetString(Resource.String.format_temperature), temp);
        }

        public static string FormatDate (long dateInMillis)
        {
            DateTime date = new DateTime(dateInMillis);
            return date.ToString("ddd, MMM dd");
        }

        public static string GetFriendlyDayString (Context context, long dateInMillis)
        {
            // The day String for forecast uses the following logic:
            // For today: "Today, June 8"
            // For tomorrow:  "Tomorrow"
            // For the next 5 days: "Wednesday" (just the day name)
            // For all days after that: "Mon Jun 8"

            var todayDate = DateTime.Now.Date;

            long currentDay = DateTime.Today.Ticks;
            var  forecastDay = new DateTime(dateInMillis).Date;
            //var forecastDay = dateAsDay.AddTicks(dateInMillis).Date;

            // If the date we're building the String for is today's date, the format
            // is "Today, June 24"
            if (todayDate == forecastDay)
            {
                string today = context.GetString(Resource.String.today);
                int formatId = Resource.String.format_full_friendly_date;
                return string.Format(context.GetString(
                    formatId),
                    today,
                    GetFormattedMonthDay(context, dateInMillis));
            }
            if (forecastDay < todayDate.AddDays(7))
            {
                // If the input date is less than a week in the future, just return the day name.
                return GetDayName(context, dateInMillis);
            }
            // Otherwise, use the form "Mon Jun 3"
            return forecastDay.ToString("ddd MMM dd");
        }

        /**
     * Given a day, returns just the name to use for that day.
     * E.g "today", "tomorrow", "wednesday".
     *
     * @param context Context to use for resource localization
     * @param dateInMillis The date in milliseconds
     * @return
     */

        public static string GetDayName (Context context, long dateInMillis)
        {
            // If the date is today, return the localized version of "Today" instead of the actual
            // day name.

            var todayDate = DateTime.Now.Date;

            DateTime dateAsDay = new DateTime();
            var forecastDay = dateAsDay.AddTicks(dateInMillis).Date;

            if (todayDate == forecastDay)
            {
                return context.GetString(Resource.String.today);
            }
            if (forecastDay == todayDate.AddDays(1))
            {
                return context.GetString(Resource.String.tomorrow);
            }
            // Otherwise, the format is just the day of the week (e.g "Wednesday".
            return forecastDay.ToString("dddd");
        }

        /**
     * Converts db date format to the format "Month day", e.g "June 24".
     * @param context Context to use for resource localization
     * @param dateInMillis The db formatted date String, expected to be of the form specified
     *                in Utility.DATE_FORMAT
     * @return The day in the form of a String formatted "December 6"
     */

        public static string GetFormattedMonthDay (Context context, long dateInMillis)
        {
            DateTime date = new DateTime(dateInMillis);
            return date.ToString("MMMM dd");
        }

        public static string GetFullFriendlyDayString (Context context, long dateInMillis)
        {

            var day = GetDayName(context, dateInMillis);
            int formatId = Resource.String.format_full_friendly_date;
            return string.Format(context.GetString(formatId),day,GetFormattedMonthDay(context, dateInMillis));

        }

        public static string GetFormattedWind (Context context, float windSpeed, float degrees)
        {
            int windFormat;
            if (IsMetric(context))
            {
                windFormat = Resource.String.format_wind_kmh;
            }
            else
            {
                windFormat = Resource.String.format_wind_mph;
                windSpeed = .621371192237334f*windSpeed;
            }

            // From wind direction in degrees, determine compass direction as a String (e.g NW)
            // You know what's fun, writing really long if/else statements with tons of possible
            // conditions.  Seriously, try it!
            string direction = "Unknown";
            if (degrees >= 337.5 || degrees < 22.5)
            {
                direction = "N";
            }
            else if (degrees >= 22.5 && degrees < 67.5)
            {
                direction = "NE";
            }
            else if (degrees >= 67.5 && degrees < 112.5)
            {
                direction = "E";
            }
            else if (degrees >= 112.5 && degrees < 157.5)
            {
                direction = "SE";
            }
            else if (degrees >= 157.5 && degrees < 202.5)
            {
                direction = "S";
            }
            else if (degrees >= 202.5 && degrees < 247.5)
            {
                direction = "SW";
            }
            else if (degrees >= 247.5 && degrees < 292.5)
            {
                direction = "W";
            }
            else if (degrees >= 292.5 || degrees < 22.5)
            {
                direction = "NW";
            }
            return string.Format(context.GetString(windFormat), windSpeed, direction);
        }

        public static bool CheckNetworkStatus(Context context)
        {
            ConnectivityManager connManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connManager.ActiveNetworkInfo;
            return (networkInfo != null && networkInfo.IsConnected);

        }

        public static int GetLocationStatus(Context context)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var status = prefs.GetInt(context.GetString(Resource.String.pref_location_status),0);
            return status;
        }

        public static void ResetLocationStatus(Context context)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutInt(context.GetString(Resource.String.pref_location_status), (int)LocationStatus.LocationStatusUnkown);
            prefsEditor.Apply();
        }

        public static bool UsingLocalGraphics (Context context)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            String sunshineArtPack = context.GetString(Resource.String.pref_art_pack_sunshine);
            return prefs.GetString(context.GetString(Resource.String.pref_art_pack_key),
                    sunshineArtPack).Equals(sunshineArtPack);
        }

        public static Object GetArtUrlForWeatherCondition(Context context, int weatherId)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var formatArtUrl = prefs.GetString(context.GetString(Resource.String.pref_art_pack_key),
                    context.GetString(Resource.String.pref_art_pack_sunshine));

            if (weatherId >= 200 && weatherId <= 232)
            {
                return string.Format(formatArtUrl, "storm");
            }
            if (weatherId >= 300 && weatherId <= 321)
            {
                return string.Format(formatArtUrl, "light_rain");
            }
            if (weatherId >= 500 && weatherId <= 504)
            {
                var test = string.Format(formatArtUrl, "rain");
                return string.Format(formatArtUrl, "rain");
            }
            if (weatherId == 511) 
            {
                return string.Format(formatArtUrl, "snow");
            }
            if (weatherId >= 520 && weatherId <= 531)
            {
                return string.Format(formatArtUrl, "rain");
            }
            if (weatherId >= 600 && weatherId <= 622)
            {
                return string.Format(formatArtUrl, "snow");
            }
            if (weatherId >= 701 && weatherId <= 761)
            {
                return string.Format(formatArtUrl, "fog");
            }
            if (weatherId == 761 || weatherId == 781)
            {
                return string.Format(formatArtUrl, "storm");
            }
            if (weatherId == 800)
            {
                return string.Format(formatArtUrl, "clear");
            }
            if (weatherId == 801)
            {
                return string.Format(formatArtUrl, "light_clouds");
            }
            if (weatherId >= 802 && weatherId <= 804)
            {
                return string.Format(formatArtUrl, "clouds");
            }
            return null;
        }

        public static int GetIconResourceForWeatherCondition (int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            if (weatherId >= 200 && weatherId <= 232) {
                return Resource.Drawable.ic_storm;
            }
            if (weatherId >= 300 && weatherId <= 321) {
                return Resource.Drawable.ic_light_rain;
            }
            if (weatherId >= 500 && weatherId <= 504) {
                return Resource.Drawable.ic_rain;
            }
            if (weatherId == 511) {
                return Resource.Drawable.ic_snow;
            }
            if (weatherId >= 520 && weatherId <= 531) {
                return Resource.Drawable.ic_rain;
            }
            if (weatherId >= 600 && weatherId <= 622) {
                return Resource.Drawable.ic_snow;
            }
            if (weatherId >= 701 && weatherId <= 761) {
                return Resource.Drawable.ic_fog;
            }
            if (weatherId == 761 || weatherId == 781) {
                return Resource.Drawable.ic_storm;
            }
            if (weatherId == 800) {
                return Resource.Drawable.ic_clear;
            }
            if (weatherId == 801) {
                return Resource.Drawable.ic_light_clouds;
            }
            if (weatherId >= 802 && weatherId <= 804) {
                return Resource.Drawable.ic_cloudy;
            }
            return -1;
        }

        public static int GetArtResourceForWeatherCondition (int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            if (weatherId >= 200 && weatherId <= 232) {
                return Resource.Drawable.art_storm;
            }
            if (weatherId >= 300 && weatherId <= 321) {
                return Resource.Drawable.art_light_rain;
            }
            if (weatherId >= 500 && weatherId <= 504) {
                return Resource.Drawable.art_rain;
            }
            if (weatherId == 511) {
                return Resource.Drawable.art_snow;
            }
            if (weatherId >= 520 && weatherId <= 531) {
                return Resource.Drawable.art_rain;
            }
            if (weatherId >= 600 && weatherId <= 622) {
                return Resource.Drawable.art_rain;
            }
            if (weatherId >= 701 && weatherId <= 761) {
                return Resource.Drawable.art_fog;
            }
            if (weatherId == 761 || weatherId == 781) {
                return Resource.Drawable.art_storm;
            }
            if (weatherId == 800) {
                return Resource.Drawable.art_clear;
            }
            if (weatherId == 801) {
                return Resource.Drawable.art_light_clouds;
            }
            if (weatherId >= 802 && weatherId <= 804) {
                return Resource.Drawable.art_clouds;
            }
            return -1;
        }

        public static string GetStringForWeatherCondition (Context context, int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            int stringId;
            if (weatherId >= 200 && weatherId <= 232)
            {
                stringId = Resource.String.condition_2xx;
            }
            else if (weatherId >= 300 && weatherId <= 321)
            {
                stringId = Resource.String.condition_3xx;
            }
            else switch (weatherId)
                {
                    case 500:
                        stringId = Resource.String.condition_500;
                        break;
                    case 501:
                        stringId = Resource.String.condition_501;
                        break;
                    case 502:
                        stringId = Resource.String.condition_502;
                        break;
                    case 503:
                        stringId = Resource.String.condition_503;
                        break;
                    case 504:
                        stringId = Resource.String.condition_504;
                        break;
                    case 511:
                        stringId = Resource.String.condition_511;
                        break;
                    case 520:
                        stringId = Resource.String.condition_520;
                        break;
                    case 531:
                        stringId = Resource.String.condition_531;
                        break;
                    case 600:
                        stringId = Resource.String.condition_600;
                        break;
                    case 601:
                        stringId = Resource.String.condition_601;
                        break;
                    case 602:
                        stringId = Resource.String.condition_602;
                        break;
                    case 611:
                        stringId = Resource.String.condition_611;
                        break;
                    case 612:
                        stringId = Resource.String.condition_612;
                        break;
                    case 615:
                        stringId = Resource.String.condition_615;
                        break;
                    case 616:
                        stringId = Resource.String.condition_616;
                        break;
                    case 620:
                        stringId = Resource.String.condition_620;
                        break;
                    case 621:
                        stringId = Resource.String.condition_621;
                        break;
                    case 622:
                        stringId = Resource.String.condition_622;
                        break;
                    case 701:
                        stringId = Resource.String.condition_701;
                        break;
                    case 711:
                        stringId = Resource.String.condition_711;
                        break;
                    case 721:
                        stringId = Resource.String.condition_721;
                        break;
                    case 731:
                        stringId = Resource.String.condition_731;
                        break;
                    case 741:
                        stringId = Resource.String.condition_741;
                        break;
                    case 751:
                        stringId = Resource.String.condition_751;
                        break;
                    case 761:
                        stringId = Resource.String.condition_761;
                        break;
                    case 762:
                        stringId = Resource.String.condition_762;
                        break;
                    case 771:
                        stringId = Resource.String.condition_771;
                        break;
                    case 781:
                        stringId = Resource.String.condition_781;
                        break;
                    case 800:
                        stringId = Resource.String.condition_800;
                        break;
                    case 801:
                        stringId = Resource.String.condition_801;
                        break;
                    case 802:
                        stringId = Resource.String.condition_802;
                        break;
                    case 803:
                        stringId = Resource.String.condition_803;
                        break;
                    case 804:
                        stringId = Resource.String.condition_804;
                        break;
                    case 900:
                        stringId = Resource.String.condition_900;
                        break;
                    case 901:
                        stringId = Resource.String.condition_901;
                        break;
                    case 902:
                        stringId = Resource.String.condition_902;
                        break;
                    case 903:
                        stringId = Resource.String.condition_903;
                        break;
                    case 904:
                        stringId = Resource.String.condition_904;
                        break;
                    case 905:
                        stringId = Resource.String.condition_905;
                        break;
                    case 906:
                        stringId = Resource.String.condition_906;
                        break;
                    case 951:
                        stringId = Resource.String.condition_951;
                        break;
                    case 952:
                        stringId = Resource.String.condition_952;
                        break;
                    case 953:
                        stringId = Resource.String.condition_953;
                        break;
                    case 954:
                        stringId = Resource.String.condition_954;
                        break;
                    case 955:
                        stringId = Resource.String.condition_955;
                        break;
                    case 956:
                        stringId = Resource.String.condition_956;
                        break;
                    case 957:
                        stringId = Resource.String.condition_957;
                        break;
                    case 958:
                        stringId = Resource.String.condition_958;
                        break;
                    case 959:
                        stringId = Resource.String.condition_959;
                        break;
                    case 960:
                        stringId = Resource.String.condition_960;
                        break;
                    case 961:
                        stringId = Resource.String.condition_961;
                        break;
                    case 962:
                        stringId = Resource.String.condition_962;
                        break;
                    default:
                        return context.GetString(Resource.String.condition_unknown, weatherId);
                }
            return context.GetString(stringId);
        }

    }
}

