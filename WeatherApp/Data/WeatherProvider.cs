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
        private UriMatcher sUriMatcher = BuildUriMatcher();
        private WeatherDbOpenHelper openHelper;
        private static Context _context = Android.App.Application.Context;
        public const int Weather = 100;
        public const int WeatherWithLocation = 101;
        public const int WeatherWithLocationAndDate = 102;
        public const int Location = 300;
        public const int ContentAuthorityId = Resource.String.content_authority;
        //public const string content_authority = context.GetString(Resource.String.content_authority);
        static string _authority = _context.GetString(Resource.String.content_authority);
        SQLiteQueryBuilder sWeatherByLocationSettingQueryBuilder = new SQLiteQueryBuilder()
        {
            //This is an inner join which looks like
            //weather INNER JOIN location ON weather.location_id = location._id
            Tables = (
                WeatherContractOpen.WeatherEntryOpen.TableName + " INNER JOIN " +
                WeatherContractOpen.LocationEntryOpen.TableName +
                " ON " + WeatherContractOpen.WeatherEntryOpen.TableName +
                "." + WeatherContractOpen.WeatherEntryOpen.ColumnLocKey +
                " = " + WeatherContractOpen.LocationEntryOpen.TableName +
                "." + WeatherContractOpen.LocationEntryOpen.Id)
        };




        //location.location_setting = ?
        private const string SLocationSettingSelection =
            WeatherContractOpen.LocationEntryOpen.TableName +
            "." + WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting + " = ? ";

        //location.location_setting = ? AND date >= ?
        private const string SLocationSettingWithStartDateSelection =
            WeatherContractOpen.LocationEntryOpen.TableName +
            "." + WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting + " = ? AND " +
            WeatherContractOpen.WeatherEntryOpen.ColumnDate + " >= ? ";

        //location.location_setting = ? AND date = ?
        private const string SLocationSettingAndDaySelection =
            WeatherContractOpen.LocationEntryOpen.TableName +
            "." + WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting + " = ? AND " +
            WeatherContractOpen.WeatherEntryOpen.ColumnDate + " = ? ";

        private ICursor GetWeatherByLocationSetting (Android.Net.Uri uri, string[] projection, string sortOrder)
        {
            var locationSetting = WeatherContractOpen.WeatherEntryOpen.GetLocationSettingFromUri(uri);
            var startDate = WeatherContractOpen.WeatherEntryOpen.GetStartDateFromUri(uri);

            string[] selectionArgs;
            string selection;

            if (startDate == 0)
            {
                selection = SLocationSettingSelection;
                selectionArgs = new string[] { locationSetting };
            }
            else
            {
                selectionArgs = new string[] { locationSetting, Long.ToString(startDate) };
                selection = SLocationSettingWithStartDateSelection;
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

        private ICursor GetWeatherByLocationSettingAndDate (
            Android.Net.Uri uri, string[] projection, string sortOrder)
        {
            var locationSetting = WeatherContractOpen.WeatherEntryOpen.GetLocationSettingFromUri(uri);
            var date = WeatherContractOpen.WeatherEntryOpen.GetDateFromUri(uri);

            return sWeatherByLocationSettingQueryBuilder.Query(openHelper.ReadableDatabase,
                projection,
                SLocationSettingAndDaySelection,
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
        public static UriMatcher BuildUriMatcher ()
        {
            // 1) The code passed into the constructor represents the code to return for the root
            // URI.  It's common to use NO_MATCH as the code for this case. Add the constructor below.
            var matcher = new UriMatcher(UriMatcher.NoMatch);
            // 2) Use the addURI function to match each of the types.  Use the constants from
            // WeatherContract to help define the types to the UriMatcher.
            matcher.AddURI(_authority, WeatherContractOpen.PathWeather, Weather);
            matcher.AddURI(_authority, WeatherContractOpen.PathWeather + "/*", WeatherWithLocation);
            matcher.AddURI(_authority, WeatherContractOpen.PathWeather + "/*/#", WeatherWithLocationAndDate);

            matcher.AddURI(_authority, WeatherContractOpen.PathLocation, Location);

            // 3) Return the new matcher!
            return matcher;
        }

        /*
        Students: We've coded this for you.  We just create a new WeatherDbHelper for later use
        here.
     */

        public override bool OnCreate ()
        {
            openHelper = new WeatherDbOpenHelper(_context);
            return true;
        }

        /*
        Students: Here's where you'll code the getType function that uses the UriMatcher.  You can
        test this by uncommenting testGetType in TestProvider.

     */

        public override string GetType (Android.Net.Uri uri)
        {

            // Use the Uri Matcher to determine what kind of URI this is.
            var match = sUriMatcher.Match(uri);

            switch (match)
            {
                // Student: Uncomment and fill out these two cases
                //            case WEATHER_WITH_LOCATION_AND_DATE:
                //            case WEATHER_WITH_LOCATION:
                case Weather:
                    return WeatherContractOpen.WeatherEntryOpen.ContentType;
                case Location:
                    return WeatherContractOpen.LocationEntryOpen.ContentType;
                case WeatherWithLocation:
                    return WeatherContractOpen.WeatherEntryOpen.ContentType;
                case WeatherWithLocationAndDate:
                    return WeatherContractOpen.WeatherEntryOpen.ContentItemType;
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
                case WeatherWithLocationAndDate:
                    {
                        retCursor = GetWeatherByLocationSettingAndDate(uri, projection, sortOrder);
                        break;
                    }
                // "weather/*"
                case WeatherWithLocation:
                    {
                        retCursor = GetWeatherByLocationSetting(uri, projection, sortOrder);
                        break;
                    }
                // "weather"
                case Weather:
                    {
                        retCursor = openHelper.ReadableDatabase.Query(WeatherContractOpen.WeatherEntryOpen.TableName, projection, selection, selectionArgs, null, null, sortOrder);
                        break;
                    }
                // "location"
                case Location:
                    {
                        retCursor = openHelper.ReadableDatabase.Query(WeatherContractOpen.LocationEntryOpen.TableName, projection, selection, selectionArgs, null, null, sortOrder);
                        break;
                    }


            }
            retCursor.SetNotificationUri(_context.ContentResolver, uri);
            return retCursor;
        }

        /*
        Student: Add the ability to insert Locations to the implementation of this function.
     */

        public override Android.Net.Uri Insert (Android.Net.Uri uri, ContentValues values)
        {
            var db = openHelper.WritableDatabase;
            var match = sUriMatcher.Match(uri);
            Android.Net.Uri returnUri;

            switch (match)
            {
                case Weather:
                    {
                        NormalizeDate(values);
                        var id = db.Insert(WeatherContractOpen.WeatherEntryOpen.TableName, null, values);
                        if (id > 0)
                            returnUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherUri(id);
                        else
                            throw new SQLException("Failed to insert row into " + uri);
                        break;
                    }
                case Location:
                    {
                        NormalizeDate(values);
                        var id = db.Insert(WeatherContractOpen.LocationEntryOpen.TableName, null, values);
                        if (id > 0)
                            returnUri = WeatherContractOpen.LocationEntryOpen.BuildLocationUri(id);
                        else
                            throw new SQLException("Failed to insert location " + uri);
                        break;
                    }
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }
            _context.ContentResolver.NotifyChange(uri, null);
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
                case Weather:
                    deletedRows = db.Delete(WeatherContractOpen.WeatherEntryOpen.TableName, selection, selectionArgs);
                    break;
                case Location:
                    deletedRows = db.Delete(WeatherContractOpen.LocationEntryOpen.TableName, selection, selectionArgs);
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
                _context.ContentResolver.NotifyChange(uri, null);
            return deletedRows;
        }

        private void NormalizeDate (ContentValues values)
        {
            // normalize the date value
            if (values.ContainsKey(WeatherContractOpen.WeatherEntryOpen.ColumnDate))
            {
                var dateValue = values.GetAsLong(WeatherContractOpen.WeatherEntryOpen.ColumnDate);
                values.Put(WeatherContractOpen.WeatherEntryOpen.ColumnDate, WeatherContractOpen.NormalizeDate(dateValue));
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
                case Weather:
                    updatedRows = db.Update(WeatherContractOpen.WeatherEntryOpen.TableName, values, selection, selectionArgs);
                    break;
                case Location:
                    updatedRows = db.Update(WeatherContractOpen.LocationEntryOpen.TableName, values, selection, selectionArgs);
                    break;
                default:
                    throw new UnsupportedOperationException("Unknown uri: " + uri);
            }

            if (updatedRows > 0 || selection == null)
                _context.ContentResolver.NotifyChange(uri, null);
            return updatedRows;
        }


        public override int BulkInsert (Android.Net.Uri uri, ContentValues[] values)
        {
            var db = openHelper.WritableDatabase;
            var match = sUriMatcher.Match(uri);
            switch (match)
            {
                case Weather:
                    db.BeginTransaction();
                    var returnCount = 0;
                    try
                    {
                        foreach (var value in values)
                        {
                            NormalizeDate(value);
                            var id = db.Insert(WeatherContractOpen.WeatherEntryOpen.TableName, null, value);
                            if (id != -1)
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
                    _context.ContentResolver.NotifyChange(uri, null);
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

