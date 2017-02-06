using Android.Net;
using System;
using Android.Content;
using System.Globalization;

namespace WeatherApp
{
    public class WeatherContractOpen
    {
        public WeatherContractOpen ()
        {
        }


        // The "Content authority" is a name for the entire content provider, similar to the
        // relationship between a domain name and its website.  A convenient string to use for the
        // content authority is the package name for the app, which is guaranteed to be unique on the
        // device.
        public const string ContentAuthority = "WeatherApp";

        // Use CONTENT_AUTHORITY to create the base of all URI's which apps will use to contact
        // the content provider.
        public static Android.Net.Uri BaseContentUri = Android.Net.Uri.Parse("content://" + ContentAuthority);

        // Possible paths (appended to base content URI for possible URI's)
        // For instance, content://com.example.android.sunshine.app/weather/ is a valid path for
        // looking at weather data. content://com.example.android.sunshine.app/givemeroot/ will fail,
        // as the ContentProvider hasn't been given any information on what to do with "givemeroot".
        // At least, let's hope not.  Don't be that dev, reader.  Don't be that dev.
        public const string PathWeather = "weather";
        public const string PathLocation = "location";

        readonly static JulianCalendar Cal = new JulianCalendar();

        public static long NormalizeDate (long startDate)
        {
            // normalize the start date to the beginning of the (UTC) day
            var time = new DateTime();
            time = time.AddTicks(startDate);
            return time.Ticks;
        }

        public class LocationEntryOpen : BaseColumns
        {
            public static Android.Net.Uri ContentUri =
                BaseContentUri.BuildUpon().AppendPath(PathLocation).Build();

            public const String ContentType =
                ContentResolver.CursorDirBaseType + "/" + ContentAuthority + "/" + PathLocation;
            public const String ContentItemType =
                ContentResolver.CursorItemBaseType + "/" + ContentAuthority + "/" + PathLocation;

            public static Android.Net.Uri BuildLocationUri (long id)
            {
                return ContentUris.WithAppendedId(ContentUri, id);
            }

            public const string TableName = "location";
            public const string ColumnLocationSetting = "location_setting";
            public const string ColumnCityName = "city_name";
            public const string ColumnCoordLat = "coord_lat";
            public const string ColumnCoordLong = "coord_long";
        }

        /* Inner class that defines the table contents of the weather table */
        public class WeatherEntryOpen : BaseColumns
        {
            public static Android.Net.Uri ContentUri =
                BaseContentUri.BuildUpon().AppendPath(PathWeather).Build();

            public static string ContentType =
                ContentResolver.CursorDirBaseType + "/" + ContentAuthority + "/" + PathWeather;
            public static string ContentItemType =
                ContentResolver.CursorItemBaseType + "/" + ContentAuthority + "/" + PathWeather;


            public static Android.Net.Uri BuildWeatherUri (long id)
            {
                return ContentUris.WithAppendedId(ContentUri, id);
            }

            /*
            Student: Fill in this BuildWeatherLocation function
         */
            public static Android.Net.Uri BuildWeatherLocation (String locationSetting)
            {
                return ContentUri.BuildUpon().AppendPath(locationSetting).Build();
            }

            public static Android.Net.Uri BuildWeatherLocationWithStartDate (
                String locationSetting, long startDate)
            {
                var normalizedDate = NormalizeDate(startDate);
                return ContentUri.BuildUpon().AppendPath(locationSetting)
                    .AppendQueryParameter(ColumnDate, normalizedDate.ToString()).Build();
            }

            public static Android.Net.Uri BuildWeatherLocationWithDate (String locationSetting, long date)
            {
                return ContentUri.BuildUpon().AppendPath(locationSetting)
                    .AppendQueryParameter(ColumnDate, NormalizeDate(date).ToString()).Build();
            }

            public static string GetLocationSettingFromUri (Android.Net.Uri uri)
            {
                return uri.PathSegments[1];
            }

            public static long GetDateFromUri (Android.Net.Uri uri)
            {
                if (uri.PathSegments.Count > 2)
                {
                    return long.Parse(uri.PathSegments[2]);
                }
                else
                {
                    var pathParts = uri.Query.Split(new[] { "date=" }, StringSplitOptions.None);
                    return long.Parse(pathParts[1]);
                }
            }

            public static long GetStartDateFromUri (Android.Net.Uri uri)
            {
                var dateString = uri.GetQueryParameter(ColumnDate);
                if (null != dateString && dateString.Length > 0)
                    return long.Parse(dateString);
                else
                    return 0;
            }

            public const string TableName = "weather";

            // Column with the foreign key into the location table.
            public const string ColumnLocKey = "location_id";
            // Date, stored as long in milliseconds since the epoch
            public const string ColumnDate = "date";
            // Weather id as returned by API, to identify the icon to be used
            public const string ColumnWeatherId = "weather_id";

            // Short description and long description of the weather, as provided by API.
            // e.g "clear" vs "sky is clear".
            public const string ColumnShortDesc = "short_desc";

            // Min and max temperatures for the day (stored as floats)
            public const string ColumnMinTemp = "min";
            public const string ColumnMaxTemp = "max";

            // Humidity is stored as a float representing percentage
            public const string ColumnHumidity = "humidity";

            // Humidity is stored as a float representing percentage
            public const string ColumnPressure = "pressure";

            // Windspeed is stored as a float representing windspeed  mph
            public const string ColumnWindSpeed = "wind";

            // Degrees are meteorological degrees (e.g, 0 is north, 180 is south).  Stored as floats.
            public const string ColumnDegrees = "degrees";
        }
    }
}

