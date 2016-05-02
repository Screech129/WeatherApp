using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Org.Json;
using System.Collections;
using Android.Preferences;
using Android.Database;


namespace WeatherApp.Sync
{

    public class SunshineSyncAdapter : AbstractThreadedSyncAdapter
    {
        private Context _context = Application.Context;
        public const string LOCATION_QUERY_EXTRA = "lqe";
        // Interval at which to sync with the weather, in milliseconds.
        // 60 seconds (1 minute)  180 = 3 hours
        public const int SYNC_INTERVAL = 60 * 180;
        public const int SYNC_FLEXTIME = SYNC_INTERVAL / 3;
        private const long Day_In_Milis = 1000 * 60 * 60 * 24;
        private const int Weather_Notification_ID = 3004;
        private static string[] Notify_Weather_Projection = new string[]
        {
            WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC
        };

        private const int Index_Weather_ID = 0;
        private const int Index_Max_Temp = 1;
        private const int Index_Min_Temp = 2;
        private const int Index_Short_Desc = 3;

        public SunshineSyncAdapter (Context context, bool autoInitialize) :
            base(context, autoInitialize)
        {

        }

        public override void OnPerformSync (Account account, Bundle extras, string authority, ContentProviderClient provider, SyncResult syncResult)
        {
            Log.Debug("SunshineSyncAdapter", "OnPerformSync called.");

            StreamReader reader = null;
            string zipCode = Utility.getPreferredLocation(_context);
             Log.Debug("SunshineSyncAdapter", "Time: " + DateTime.Now + "Zip: " + zipCode);
            try
            {

                var httpClient = new HttpClient();
                // Construct the URL for the OpenWeatherMap query
                // Possible parameters are available at OWM's forecast API page, at
                // http://openweathermap.org/API#forecast
                Task<string> getJSON = httpClient.GetStringAsync("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + zipCode + ",us&mode=json&units=metric&cnt=7&APPID=83fde89b086ca4abec16cb2a8c245bb8");
                string JSON = getJSON.Result;
                GetWeatherDataFromJson(JSON, zipCode);
                DeleteOldWeather();
                NotifyWeather();
            }
            catch (IOException e)
            {
                Log.WriteLine(LogPriority.Error, "PlaceholderFragment", "Error ", e);
                // If the code didn't successfully get the weather data, there's no point in attempting
                // to parse it.
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e)
                    {
                        Log.WriteLine(LogPriority.Error, "PlaceholderFragment", "Error closing stream", e);
                    }
                }

            }
        }

        private void DeleteOldWeather ()
        {
            var yesterday = DateTime.Today.AddDays(-1).Ticks;
            var formattedDate = WeatherContractOpen.normalizeDate(yesterday);
            var deletedCount = _context.ContentResolver.Delete(WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE +  "<= ?", new String[] { formattedDate.ToString() });
            Log.Debug("Fetch Weather Task", "FetchweatherTask Complete " + deletedCount);
        }

        /**
    * Helper method to have the sync adapter sync immediately
    * @param context The context used to access the account service
    */
        public static void SyncImmediately (Context context)
        {
            Bundle bundle = new Bundle();
            bundle.PutBoolean(ContentResolver.SyncExtrasExpedited, true);
            bundle.PutBoolean(ContentResolver.SyncExtrasManual, true);
            ContentResolver.RequestSync(GetSyncAccount(context),
                    context.GetString(Resource.String.content_authority), bundle);

        }

        /**
         * Helper method to get the fake account to be used with SyncAdapter, or make a new one
         * if the fake account doesn't exist yet.  If we make a new account, we call the
         * onAccountCreated method so we can initialize things.
         *
         * @param context The context used to access the account service
         * @return a fake account.
         */
        public static Account GetSyncAccount (Context context)
        {
            // Get an instance of the Android account manager
            AccountManager accountManager =
                    (AccountManager)context.GetSystemService(Context.AccountService);

            // Create the account type and default account
            Account newAccount = new Account(
                    context.GetString(Resource.String.app_name), context.GetString(Resource.String.sync_account_type));

            // If the password doesn't exist, the account doesn't exist
            if (null == accountManager.GetPassword(newAccount))
            {

                /*
                 * Add the account and account type, no password or user data
                 * If successful, return the Account object, otherwise report an error.
                 */
                if (!accountManager.AddAccountExplicitly(newAccount, "", null))
                {
                    return null;
                }
                /*
                 * If you don't set android:syncable="true" in
                 * in your <provider> element in the manifest,
                 * then call ContentResolver.SetIsSyncable(account, AUTHORITY, 1)
                 * here.
                 */
                OnAccountCreated(newAccount,context);
            }
            return newAccount;
        }

        public void GetWeatherDataFromJson (string forecastJsonStr, string locationSetting)
        {

            // These are the names of the JSON objects that need to be extracted.
            // Location information
            const string OWM_CITY = "city";
            const string OWM_CITY_NAME = "name";
            const string OWM_COORD = "coord";

            const string OWM_LATITUDE = "lat";
            const string OWM_LONGITUDE = "lon";

            const string OWM_LIST = "list";

            const string OWM_PRESSURE = "pressure";
            const string OWM_HUMIDITY = "humidity";
            const string OWM_WINDSPEED = "speed";
            const string OWM_WIND_DIRECTION = "deg";

            const string OWM_TEMPERATURE = "temp";
            const string OWM_MAX = "max";
            const string OWM_MIN = "min";

            const string OWM_WEATHER = "weather";
            const string OWM_DESCRIPTION = "main";
            const string OWM_WEATHER_id = "id";

            try
            {
                JSONObject forecastJson = new JSONObject(forecastJsonStr);
                JSONArray weatherArray = forecastJson.GetJSONArray(OWM_LIST);

                JSONObject cityJson = forecastJson.GetJSONObject(OWM_CITY);
                string cityName = cityJson.GetString(OWM_CITY_NAME);

                JSONObject cityCoord = cityJson.GetJSONObject(OWM_COORD);
                double cityLatitude = cityCoord.GetDouble(OWM_LATITUDE);
                double cityLongitude = cityCoord.GetDouble(OWM_LONGITUDE);

                long locationId = AddLocation(locationSetting, cityName, cityLatitude, cityLongitude);

                ArrayList jsonResultValues = new ArrayList();

                DateTime dayTime = DateTime.UtcNow;


                for (int i = 0; i < weatherArray.Length(); i++)
                {
                    // These are the values that will be collected.
                    long dateTime;
                    double pressure;
                    int humidity;
                    double windSpeed;
                    double windDirection;

                    double high;
                    double low;

                    string description;
                    int weatherId;

                    // Get the JSON object representing the day
                    JSONObject dayForecast = weatherArray.GetJSONObject(i);

                    // The date/time is returned as a long.  We need to convert that
                    // into something human-readable, since most people won't read "1400356800" as
                    // "this saturday".
                    // Cheating to convert this to UTC time, which is what we want anyhow
                    dateTime = dayTime.AddDays(i).Ticks;

                    pressure = dayForecast.GetDouble(OWM_PRESSURE);
                    humidity = dayForecast.GetInt(OWM_HUMIDITY);
                    windSpeed = dayForecast.GetDouble(OWM_WINDSPEED);
                    windDirection = dayForecast.GetDouble(OWM_WIND_DIRECTION);

                    // Description is in a child array called "weather", which is 1 element long.
                    // That element also contains a weather code.
                    JSONObject weatherObject =
                        dayForecast.GetJSONArray(OWM_WEATHER).GetJSONObject(0);
                    description = weatherObject.GetString(OWM_DESCRIPTION);
                    weatherId = weatherObject.GetInt(OWM_WEATHER_id);

                    // Temperatures are in a child object called "temp".  Try not to name variables
                    // "temp" when working with temperature.  It confuses everybody.
                    JSONObject temperatureObject = dayForecast.GetJSONObject(OWM_TEMPERATURE);
                    high = temperatureObject.GetDouble(OWM_MAX);
                    low = temperatureObject.GetDouble(OWM_MIN);

                    ContentValues weatherValues = new ContentValues();

                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_LOC_KEY, locationId);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE, dateTime);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY, humidity);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE, pressure);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED, windSpeed);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES, windDirection);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP, high);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP, low);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC, description);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id, weatherId);

                    jsonResultValues.Add(weatherValues);
                }
                BulkInsertWeather(jsonResultValues, locationSetting);

            }
            catch (Exception ex)
            {
                Log.Error("Featch Weather Task", ex.Message);
            }


        }

        public long AddLocation (string locationSetting, string cityName, double lat, double lon)
        {
            var returnedId = "0";

            var location = _context.ContentResolver.Query(WeatherContractOpen.LocationEntryOpen.CONTENT_URI, null, WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING + " = ?", new string[] { locationSetting }, null);
            if (!location.MoveToFirst())
            {
                var locationValues = CreateLocationValues(locationSetting, cityName, lat, lon);
                var insertUri = _context.ContentResolver.Insert(WeatherContractOpen.LocationEntryOpen.CONTENT_URI, locationValues);
                returnedId = insertUri.LastPathSegment;
            }
            else
            {
                var locationIdColumn = location.GetColumnIndex(WeatherContractOpen.LocationEntryOpen._ID);
                returnedId = location.GetInt(locationIdColumn).ToString();
            }

            return long.Parse(returnedId);
        }


        public void BulkInsertWeather (ArrayList weatherValues, string locationSetting)
        {
            _context.ContentResolver.Delete(WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, null, null);
            var insertedCount = 0;
            if (weatherValues.Count > 0)
            {
                var cvArray = new ContentValues[weatherValues.Count];

                cvArray = (ContentValues[])weatherValues.ToArray(typeof(ContentValues));

                insertedCount = _context.ContentResolver.BulkInsert(WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, cvArray);
            }

            Log.Debug("Fetch Weather Task", "FetchweatherTask Complete " + insertedCount + " records Inserted");
        }

        public static ContentValues CreateLocationValues (string locationSetting, string cityName, double lat, double lon)
        {

            // Create a new map of values, where column names are the keys
            ContentValues locValues = new ContentValues();
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING, locationSetting);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME, cityName);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT, lat);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG, lon);

            return locValues;
        }

        /**
    * Helper method to schedule the sync adapter periodic execution
    */
        public static void ConfigurePeriodicSync (Context context, int syncInterval, int flexTime)
        {
            Account account = GetSyncAccount(context);
            string authority = context.GetString(Resource.String.content_authority);
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat)
            {
                // we can enable inexact timers in our periodic sync
                SyncRequest request = new SyncRequest.Builder().
                        SyncPeriodic(syncInterval, flexTime).
                        SetSyncAdapter(account, authority).
                        SetExtras(new Bundle()).Build();
                ContentResolver.RequestSync(request);
            }
            else
            {
                ContentResolver.AddPeriodicSync(account,
                        authority, new Bundle(), syncInterval);
            }
        }


        private static void OnAccountCreated (Account newAccount, Context context)
        {
            /*
             * Since we've created an account
             */
            ConfigurePeriodicSync(context, SYNC_INTERVAL, SYNC_FLEXTIME);

            /*
             * Without calling setSyncAutomatically, our periodic sync will not be enabled.
             */
            ContentResolver.SetSyncAutomatically(newAccount, context.GetString(Resource.String.content_authority), true);

            /*
             * Finally, let's do a sync to get things started
             */
            SyncImmediately(context);
        }

        public static void InitializeSyncAdapter (Context context)
        {
            GetSyncAccount(context);
        }

        private void NotifyWeather ()
        {
            var currTimeMilli = DateTime.Now.Ticks;
            //checking the last update and notify if it' the first of the day
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            string lastNotificationKey = _context.GetString(Resource.String.pref_last_notification);
            long lastSync = prefs.GetLong(lastNotificationKey, 0);
            var notificationEnableKey = _context.GetString(Resource.String.pref_enable_notifications_key);
            var notificationEnabled = prefs.GetBoolean(notificationEnableKey, true);
            if (currTimeMilli - lastSync >= Day_In_Milis && notificationEnabled)
            {
                // Last sync was more than 1 day ago, let's send a notification with the weather.
                string locationQuery = Utility.getPreferredLocation(_context);

                Android.Net.Uri weatherUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate(locationQuery, currTimeMilli);

                // we'll query our contentProvider, as always
                ICursor cursor = _context.ContentResolver.Query(weatherUri, Notify_Weather_Projection, null, null, null);

                if (cursor.MoveToFirst())
                {
                    int weatherId = cursor.GetInt(Index_Weather_ID);
                    double high = cursor.GetDouble(Index_Max_Temp);
                    double low = cursor.GetDouble(Index_Min_Temp);
                    string desc = cursor.GetString(Index_Short_Desc);

                    int iconId = Utility.getIconResourceForWeatherCondition(weatherId);
                    string title = _context.GetString(Resource.String.app_name);
                    bool isMetric = Utility.isMetric(_context);

                    // Define the text of the forecast.
                    string contentText = string.Format(_context.GetString(Resource.String.format_notification),
                            desc,
                            Utility.formatTemperature(_context, high,isMetric),
                            Utility.formatTemperature(_context, low,isMetric));

                    //build your notification here.
                    Notification.Builder builder = new Notification.Builder(_context)
                   .SetSmallIcon(iconId)
                   .SetContentTitle("Today's Weather")
                   .SetContentText(contentText);

                    Intent notificationIntent = new Intent(_context, typeof(MainActivity));

                    TaskStackBuilder stackBuilder = TaskStackBuilder.Create(_context);
                    stackBuilder.AddParentStack(Java.Lang.Class.FromType (typeof(MainActivity)));
                    stackBuilder.AddNextIntent(notificationIntent);

                    PendingIntent notificationPendIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

                    builder.SetContentIntent(notificationPendIntent);

                    NotificationManager notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                    notificationManager.Notify(Weather_Notification_ID, builder.Build());

                    //refreshing last sync
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutLong(lastNotificationKey, currTimeMilli);
                    editor.Commit();
                }
            }

        }

    }
}