using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
using Java.Lang;
using Android.Net;

namespace WeatherApp
{
    [ContentProvider(new string[] { "WeatherApp"}, Exported = true, Syncable = true)]
    public class WeatherProvider : ContentProvider
    {
        public WeatherProvider ()
            :base()
        {
        }


        // The URI Matcher used by this content provider.
        private UriMatcher sUriMatcher = buildUriMatcher();
        private WeatherDbOpenHelper openHelper;
        private static Context context = Android.App.Application.Context;
        public const int WEATHER = 100;
        public const int WEATHER_WITH_LOCATION = 101;
        public const int WEATHER_WITH_LOCATION_AND_DATE = 102;
        public const int LOCATION = 300;
        public const int content_authority_id = Resource.String.content_authority;
        //public const string content_authority = context.GetString(Resource.String.content_authority);
        static string AUTHORITY = context.GetString(Resource.String.content_authority);
        SQLiteQueryBuilder sWeatherByLocationSettingQueryBuilder = new SQLiteQueryBuilder()
        {
            //This is an inner join which looks like
            //weather INNER JOIN location ON weather.location_id = location._id
            Tables = (
                WeatherContractOpen.WeatherEntryOpen.TABLE_NAME + " INNER JOIN " +
                WeatherContractOpen.LocationEntryOpen.TABLE_NAME +
                " ON " + WeatherContractOpen.WeatherEntryOpen.TABLE_NAME +
                "." + WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY +
                " = " + WeatherContractOpen.LocationEntryOpen.TABLE_NAME +
                "." + WeatherContractOpen.LocationEntryOpen._ID)
        };




        //location.location_setting = ?
        private const string sLocationSettingSelection =
            WeatherContractOpen.LocationEntryOpen.TABLE_NAME +
            "." + WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ? ";

        //location.location_setting = ? AND date >= ?
        private const string sLocationSettingWithStartDateSelection =
            WeatherContractOpen.LocationEntryOpen.TABLE_NAME +
            "." + WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ? AND " +
            WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " >= ? ";

        //location.location_setting = ? AND date = ?
        private const string sLocationSettingAndDaySelection =
            WeatherContractOpen.LocationEntryOpen.TABLE_NAME +
            "." + WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ? AND " +
            WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE + " = ? ";

        private ICursor getWeatherByLocationSetting (Android.Net.Uri uri, string[] projection, string sortOrder)
        {
            string locationSetting = WeatherContractOpen.WeatherEntryOpen.getLocationSettingFromUri(uri);
            long startDate = WeatherContractOpen.WeatherEntryOpen.getStartDateFromUri(uri);

            string[] selectionArgs;
            string selection;

            if (startDate == 0)
            {
                selection = sLocationSettingSelection;
                selectionArgs = new string[] { locationSetting };
            }
            else
            {
                selectionArgs = new string[] { locationSetting, Long.ToString(startDate) };
                selection = sLocationSettingWithStartDateSelection;
            }

            return sWeatherByLocationSettingQueryBuilder.Query(openHelper.ReadableDatabase,
                projection,
                selection,
                selectionArgs,
                null,
                null,
                sortOrder
            );
        }

        private ICursor getWeatherByLocationSettingAndDate (
            Android.Net.Uri uri, string[] projection, string sortOrder)
        {
            string locationSetting = WeatherContractOpen.WeatherEntryOpen.getLocationSettingFromUri(uri);
            long date = WeatherContractOpen.WeatherEntryOpen.getDateFromUri(uri);

            return sWeatherByLocationSettingQueryBuilder.Query(openHelper.ReadableDatabase,
                projection,
                sLocationSettingAndDaySelection,
                new string[] { locationSetting, Long.ToString(date) },
                null,
                null,
                sortOrder
            );
        }

        /*
        Students: Here is where you need to create the UriMatcher. This UriMatcher will
        match each URI to the WEATHER, WEATHER_WITH_LOCATION, WEATHER_WITH_LOCATION_AND_DATE,
        and LOCATION integer constants defined above.  You can test this by uncommenting the
        testUriMatcher test within TestUriMatcher.
     */
        public static UriMatcher buildUriMatcher ()
        {
            // 1) The code passed into the constructor represents the code to return for the root
            // URI.  It's common to use NO_MATCH as the code for this case. Add the constructor below.
            var matcher = new UriMatcher(UriMatcher.NoMatch);
            // 2) Use the addURI function to match each of the types.  Use the constants from
            // WeatherContract to help define the types to the UriMatcher.
            matcher.AddURI(AUTHORITY, WeatherContractOpen.PATH_WEATHER, WEATHER);
            matcher.AddURI(AUTHORITY, WeatherContractOpen.PATH_WEATHER + "/*", WEATHER_WITH_LOCATION);
            matcher.AddURI(AUTHORITY, WeatherContractOpen.PATH_WEATHER + "/*/#", WEATHER_WITH_LOCATION_AND_DATE);

            matcher.AddURI(AUTHORITY, WeatherContractOpen.PATH_LOCATION, LOCATION);

            // 3) Return the new matcher!
            return matcher;
        }

        /*
        Students: We've coded this for you.  We just create a new WeatherDbHelper for later use
        here.
     */

        public override bool OnCreate ()
        {
            openHelper = new WeatherDbOpenHelper(context);
            return true;
        }

        /*
        Students: Here's where you'll code the getType function that uses the UriMatcher.  You can
        test this by uncommenting testGetType in TestProvider.

     */

        public override string GetType (Android.Net.Uri uri)
        {

            // Use the Uri Matcher to determine what kind of URI this is.
            int match = sUriMatcher.Match(uri);

            switch (match)
            {
                // Student: Uncomment and fill out these two cases
                //            case WEATHER_WITH_LOCATION_AND_DATE:
                //            case WEATHER_WITH_LOCATION:
                case WEATHER:
                    return WeatherContractOpen.WeatherEntryOpen.CONTENT_TYPE;
                case LOCATION:
                    return WeatherContractOpen.LocationEntryOpen.CONTENT_TYPE;
                case WEATHER_WITH_LOCATION:
                    return WeatherContractOpen.WeatherEntryOpen.CONTENT_TYPE;
                case WEATHER_WITH_LOCATION_AND_DATE:
                    return WeatherContractOpen.WeatherEntryOpen.CONTENT_ITEM_TYPE;
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }
        }

        public override ICursor Query (Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs,
                                       string sortOrder)
        {
            // Here's the switch statement that, given a URI, will determine what kind of request it is,
            // and query the database accordingly.
            ICursor retCursor = null;
            switch (sUriMatcher.Match(uri))
            {
                // "weather/*/*"
                case WEATHER_WITH_LOCATION_AND_DATE:
                    {
                        retCursor = getWeatherByLocationSettingAndDate(uri, projection, sortOrder);
                        break;
                    }
                // "weather/*"
                case WEATHER_WITH_LOCATION:
                    {
                        retCursor = getWeatherByLocationSetting(uri, projection, sortOrder);
                        break;
                    }
                // "weather"
                case WEATHER:
                    {
                        retCursor = openHelper.ReadableDatabase.Query(WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, projection, selection, selectionArgs, null, null, sortOrder);
                        break;
                    }
                // "location"
                case LOCATION:
                    {
                        retCursor = openHelper.ReadableDatabase.Query(WeatherContractOpen.LocationEntryOpen.TABLE_NAME, projection, selection, selectionArgs, null, null, sortOrder);
                        break;
                    }


            }
            retCursor.SetNotificationUri(context.ContentResolver, uri);
            return retCursor;
        }

        /*
        Student: Add the ability to insert Locations to the implementation of this function.
     */

        public override Android.Net.Uri Insert (Android.Net.Uri uri, ContentValues values)
        {
            SQLiteDatabase db = openHelper.WritableDatabase;
            int match = sUriMatcher.Match(uri);
            Android.Net.Uri returnUri;

            switch (match)
            {
                case WEATHER:
                    {
                        normalizeDate(values);
                        long _id = db.Insert(WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, null, values);
                        if (_id > 0)
                            returnUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherUri(_id);
                        else
                            throw new SQLException("Failed to insert row into " + uri);
                        break;
                    }
                case LOCATION:
                    {
                        normalizeDate(values);
                        long _id = db.Insert(WeatherContractOpen.LocationEntryOpen.TABLE_NAME, null, values);
                        if (_id > 0)
                            returnUri = WeatherContractOpen.LocationEntryOpen.buildLocationUri(_id);
                        else
                            throw new SQLException("Failed to insert location " + uri);
                        break;
                    }
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }
            context.ContentResolver.NotifyChange(uri, null);
            return returnUri;
        }

        public override int Delete (Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            // Student: Start by getting a writable database
            var db = openHelper.WritableDatabase;
            Android.Net.Uri returnUri;
            var deletedRows = 0;
            // Student: Use the uriMatcher to match the WEATHER and LOCATION URI's we are going to
            // handle.  If it doesn't match these, throw an UnsupportedOperationException.
            var match = sUriMatcher.Match(uri);
            if (selection == null)
                selection = "1";
            switch (match)
            {
                case WEATHER:
                    deletedRows = db.Delete(WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, selection, selectionArgs);
                    break;
                case LOCATION:
                    deletedRows = db.Delete(WeatherContractOpen.LocationEntryOpen.TABLE_NAME, selection, selectionArgs);
                    break;
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }
            // Student: A null value deletes all rows.  In my implementation of this, I only notified
            // the uri listeners (using the content resolver) if the rowsDeleted != 0 or the selection
            // is null.
            // Oh, and you should notify the listeners here.

            // Student: return the actual rows deleted
            if (deletedRows > 0 || selection == null)
                context.ContentResolver.NotifyChange(uri, null);
            return deletedRows;
        }

        private void normalizeDate (ContentValues values)
        {
            // normalize the date value
            if (values.ContainsKey(WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE))
            {
                long dateValue = values.GetAsLong(WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE);
                values.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE, WeatherContractOpen.normalizeDate(dateValue));
            }
        }


        public override int Update (Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            var db = openHelper.WritableDatabase;
            Android.Net.Uri returnUri;
            var updatedRows = 0;
            var match = sUriMatcher.Match(uri);
            // Student: This is a lot like the delete function.  We return the number of rows impacted
            // by the update.
            switch (match)
            {
                case WEATHER:
                    updatedRows = db.Update(WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, values, selection, selectionArgs);
                    break;
                case LOCATION:
                    updatedRows = db.Update(WeatherContractOpen.LocationEntryOpen.TABLE_NAME, values, selection, selectionArgs);
                    break;
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }

            if (updatedRows > 0 || selection == null)
                context.ContentResolver.NotifyChange(uri, null);
            return updatedRows;
        }


        public override int BulkInsert (Android.Net.Uri uri, ContentValues[] values)
        {
            SQLiteDatabase db = openHelper.WritableDatabase;
            int match = sUriMatcher.Match(uri);
            switch (match)
            {
                case WEATHER:
                    db.BeginTransaction();
                    int returnCount = 0;
                    try
                    {
                        foreach (ContentValues value in values)
                        {
                            normalizeDate(value);
                            long _id = db.Insert(WeatherContractOpen.WeatherEntryOpen.TABLE_NAME, null, value);
                            if (_id != -1)
                            {
                                returnCount++;
                            }
                        }
                        db.SetTransactionSuccessful();
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                    context.ContentResolver.NotifyChange(uri, null);
                    return returnCount;
                default:
                    return base.BulkInsert(uri, values);
            }
        }

        // You do not need to call this method. This is a method specifically to assist the testing
        // framework in running smoothly. You can read more at:
        // http://developer.android.com/reference/android/content/ContentProvider.html#shutdown()

        public override void Shutdown ()
        {
            openHelper.Close();
            base.Shutdown();
        }
    }

    public class MyContentProvider : ContentProviderAttribute
    {
        public static string[] authorities { get; protected set; }
        private static Context _context = Android.App.Application.Context;
        public MyContentProvider (int resourceId)
            : base(new string[] { "WeatherApp" })
        {
            var auth = new string[] { _context.GetString(resourceId) };
            authorities = auth;
            
            typeof(ContentProviderAttribute).GetProperty("Authorities").SetValue(this, authorities);
        }
    }
}

