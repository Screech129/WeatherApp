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
using Android.Content.Res;
using Android.Preferences;
using Android.Database;
using Android.Graphics;
using Android.Views.Animations;
using Com.Bumptech.Glide;
using Java.Lang;
using Java.Util.Concurrent;
using Exception = System.Exception;
using String = System.String;
using Java.Net;

namespace WeatherApp.Sync
{

    public class SunshineSyncAdapter : AbstractThreadedSyncAdapter
    {
        private Context context = Application.Context;
        // Interval at which to sync with the weather, in milliseconds.
        // 60 seconds (1 minute)  180 = 3 hours
        public const int SyncInterval = 60 * 180;
        public const int SyncFlextime = SyncInterval / 3;
        private const long DayInMilis = 1000 * 60 * 60 * 24;
        private const int WeatherNotificationId = 3004;

        private static string[] _notifyWeatherProjection = new string[]
        {
            WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId,
            WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp,
            WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp,
            WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc
        };

        private const int IndexWeatherId = 0;
        private const int IndexMaxTemp = 1;
        private const int IndexMinTemp = 2;
        private const int IndexShortDesc = 3;

        public SunshineSyncAdapter (Context context, bool autoInitialize) :
            base(context, autoInitialize)
        {

        }

        public override void OnPerformSync (Account account, Bundle extras, string authority, ContentProviderClient provider, SyncResult syncResult)
        {
            var locationQuery = Utility.GetPreferredLocation(context);
            Log.Debug("SunshineSyncAdapter", "Starting Sync Time: " + DateTime.Now + "Zip: " + locationQuery);


            string forecastJsonStr = null;

            var format = "json";
            var units = "metric";
            var numDays = 14;
            var httpClient = new HttpClient();

            try
            {
                // Construct the URL for the OpenWeatherMap query
                // Possible parameters are available at OWM's forecast API page, at
                // http://openweathermap.org/API#forecast
                //http://api.openweathermap.org/data/2.5/forecast/daily?q=
                var uri = "http://api.openweathermap.org/data/2.5/forecast/daily?zip=" + locationQuery + ",us&mode=" + format + "&units=" + units + "&cnt=" + numDays.ToString() + "&APPID=83fde89b086ca4abec16cb2a8c245bb8";
                var getJson =
                    httpClient.GetStringAsync(uri);
                forecastJsonStr = getJson.Result;
                GetWeatherDataFromJson(forecastJsonStr, locationQuery);
            }

            catch (IOException e)
            {
                Log.WriteLine(LogPriority.Error, "PlaceholderFragment", "Error ", e);
                SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusServerDown);
            }
            catch (Exception ex)
            {
                Log.Error("Stream Error", ex.ToString());
                SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusServerDown);
            }
            finally
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                }
            }

        }

        public void GetWeatherDataFromJson (string forecastJsonStr, string locationSetting)
        {

            // These are the names of the JSON objects that need to be extracted.
            // Location information
            const string owmCity = "city";
            const string owmCityName = "name";
            const string owmCoord = "coord";

            const string owmLatitude = "lat";
            const string owmLongitude = "lon";

            const string owmList = "list";

            const string owmPressure = "pressure";
            const string owmHumidity = "humidity";
            const string owmWindspeed = "speed";
            const string owmWindDirection = "deg";

            const string owmTemperature = "temp";
            const string owmMax = "max";
            const string owmMin = "min";

            const string owmWeather = "weather";
            const string owmDescription = "main";
            const string owmWeatherId = "id";

            const string owmMessageCode = "cod";

            try
            {
                var forecastJson = new JSONObject(forecastJsonStr);

                if (forecastJson.Has(owmMessageCode))
                {
                    var errorCode = forecastJson.GetInt(owmMessageCode);

                    switch (errorCode)
                    {
                        case (int)HttpStatus.Ok:
                            break;
                        case (int)HttpStatus.NotFound:
                            SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusInvalid);
                            return;
                        default:
                            SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusServerDown);
                            return;
                    }
                }

                var weatherArray = forecastJson.GetJSONArray(owmList);
                var cityJson = forecastJson.GetJSONObject(owmCity);
                var cityName = cityJson.GetString(owmCityName);

                var cityCoord = cityJson.GetJSONObject(owmCoord);
                var cityLatitude = cityCoord.GetDouble(owmLatitude);
                var cityLongitude = cityCoord.GetDouble(owmLongitude);

                var locationId = AddLocation(locationSetting, cityName, cityLatitude, cityLongitude);

                //ArrayList jsonResultValues = new ArrayList();

                var cvArray = new ContentValues[weatherArray.Length()];


                var dayTime = DateTime.Now.Date;



                for (var i = 0; i < weatherArray.Length(); i++)
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
                    var dayForecast = weatherArray.GetJSONObject(i);

                    // The date/time is returned as a long.  We need to convert that
                    // into something human-readable, since most people won't read "1400356800" as
                    // "this saturday".
                    // Cheating to convert this to UTC time, which is what we want anyhow
                    dateTime = dayTime.AddDays(i).Ticks;

                    pressure = dayForecast.GetDouble(owmPressure);
                    humidity = dayForecast.GetInt(owmHumidity);
                    windSpeed = dayForecast.GetDouble(owmWindspeed);
                    windDirection = dayForecast.GetDouble(owmWindDirection);

                    // Description is in a child array called "weather", which is 1 element long.
                    // That element also contains a weather code.
                    var weatherObject =
                        dayForecast.GetJSONArray(owmWeather).GetJSONObject(0);
                    description = weatherObject.GetString(owmDescription);
                    weatherId = weatherObject.GetInt(owmWeatherId);

                    // Temperatures are in a child object called "temp".  Try not to name variables
                    // "temp" when working with temperature.  It confuses everybody.
                    var temperatureObject = dayForecast.GetJSONObject(owmTemperature);
                    high = temperatureObject.GetDouble(owmMax);
                    low = temperatureObject.GetDouble(owmMin);


                    var weatherValues = new ContentValues();

                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnLocKey, locationId);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnDate, dateTime);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnHumidity, humidity);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnPressure, pressure);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnWindSpeed, windSpeed);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnDegrees, windDirection);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp, high);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp, low);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc, description);
                    weatherValues.Put(WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId, weatherId);

                    cvArray[i] = (weatherValues);
                }

                var inserted = 0;

                if (cvArray.Count() > 0)
                {
                    context.ContentResolver.Delete(WeatherContractOpen.WeatherEntryOpen.ContentUri,
                        WeatherContractOpen.WeatherEntryOpen.ColumnDate + " <= ?",
                        new string[] { dayTime.AddDays(1).Ticks.ToString() });
                    context.ContentResolver.BulkInsert(WeatherContractOpen.WeatherEntryOpen.ContentUri, cvArray);
                    

                    NotifyWeather();
                }

                Log.Debug("SunshineSyncAdapter", "Sync complete. " + cvArray.Count() + " Inserted");
                SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusOk);
                //BulkInsertWeather(cvArray, locationSetting);

            }
            catch (JSONException je)
            {
                Log.Error("SunshineSyncAdapter", je.ToString());
                SetLocationStatus(context, (int)Helpers.LocationStatus.LocationStatusServerInvalid);
            }
            catch (Exception ex)
            {
                Log.Error("Featch Weather Task", ex.Message);
            }


        }

        private void NotifyWeather ()
        {
            var currTimeMilli = DateTime.Now.Date.Ticks;
            //checking the last update and notify if it' the first of the day
            var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var notificationEnableKey = context.GetString(Resource.String.pref_enable_notifications_key);
            var notificationEnabled = prefs.GetBoolean(notificationEnableKey, true);


            var lastNotificationKey = context.GetString(Resource.String.pref_last_notification);
            var lastSync = prefs.GetLong(lastNotificationKey, 0);


            if (currTimeMilli - lastSync >= DayInMilis && notificationEnabled)
            {
                // Last sync was more than 1 day ago, let's send a notification with the weather.
                var locationQuery = Utility.GetPreferredLocation(context);

                var weatherUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithDate(locationQuery, currTimeMilli);

                // we'll query our contentProvider, as always
                var cursor = context.ContentResolver.Query(weatherUri, _notifyWeatherProjection, null, null, null);

                if (cursor != null)
                {
                    var weatherId = cursor.GetInt(IndexWeatherId);
                    var high = cursor.GetDouble(IndexMaxTemp);
                    var low = cursor.GetDouble(IndexMinTemp);
                    var desc = cursor.GetString(IndexShortDesc);

                    var iconId = Utility.GetIconResourceForWeatherCondition(weatherId);
                    var resources = Context.Resources;
                    var artResourceId = Utility.GetArtResourceForWeatherCondition(weatherId);
                    var artUrl = Utility.GetArtUrlForWeatherCondition(Context, weatherId);

                    var largeIconWidth = Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb
                        ? Android.Resource.Dimension.NotificationLargeIconWidth
                        : Resource.Dimension.notification_large_icon_default;
                    var largeIconHeight = Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb
                        ? Android.Resource.Dimension.NotificationLargeIconHeight
                        : Resource.Dimension.notification_large_icon_default;


                    Bitmap largeIcon;
                    try
                    {
                        largeIcon = (Bitmap)Glide.With(Context)
                        .Load(artUrl)
                        .BitmapTransform()
                        .Error(artResourceId)
                        .Into(largeIconWidth, largeIconHeight).Get();
                    }
                    catch (Exception ex) when (ex is InterruptedException || ex is ExecutionException)
                    {
                        Log.Error("Glide Error", "Error retrieving large icon from " + artUrl, ex);
                        largeIcon = BitmapFactory.DecodeResource(resources, artResourceId);
                    }


                    var title = context.GetString(Resource.String.app_name);
                    var isMetric = Utility.IsMetric(context);

                    // Define the text of the forecast.
                    var contentText = string.Format(context.GetString(Resource.String.format_notification),
                            desc,
                            Utility.FormatTemperature(context, high, isMetric),
                            Utility.FormatTemperature(context, low, isMetric));

                    //build your notification here.
                    var builder = new Notification.Builder(context)
                   .SetColor(resources.GetColor(Resource.Color.primary_light))
                   .SetSmallIcon(iconId)
                   .SetLargeIcon(largeIcon)
                   .SetContentTitle(title)
                   .SetContentText(contentText);

                    var notificationIntent = new Intent(context, typeof(MainActivity));

                    var stackBuilder = TaskStackBuilder.Create(context);
                    stackBuilder.AddNextIntent(notificationIntent);
                    var notificationPendIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
                    builder.SetContentIntent(notificationPendIntent);
                    //stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));



                    var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                    notificationManager.Notify(WeatherNotificationId, builder.Build());

                    //refreshing last sync
                    var editor = prefs.Edit();
                    editor.PutLong(lastNotificationKey, currTimeMilli);
                    editor.Commit();
                }
                cursor.Close();
            }

        }

        public long AddLocation (string locationSetting, string cityName, double lat, double lon)
        {
            long locationId;
            var location = context.ContentResolver.Query(WeatherContractOpen.LocationEntryOpen.ContentUri, new String[] { BaseColumns.Id },
                WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting + " = ?",
                new string[] { locationSetting },
                null);

            if (location.MoveToFirst())
            {
                var locationIdIndex = location.GetColumnIndex(BaseColumns.Id);
                locationId = location.GetLong(locationIdIndex);
            }
            else
            {
                // Now that the content provider is set up, inserting rows of data is pretty simple.
                // First create a ContentValues object to hold the data you want to insert.
                var locationValues = new ContentValues();

                // Then add the data, along with the corresponding name of the data type,
                // so the content provider knows what kind of value is being inserted.
                locationValues.Put(WeatherContractOpen.LocationEntryOpen.ColumnCityName, cityName);
                locationValues.Put(WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting, locationSetting);
                locationValues.Put(WeatherContractOpen.LocationEntryOpen.ColumnCoordLat, lat);
                locationValues.Put(WeatherContractOpen.LocationEntryOpen.ColumnCoordLong, lon);

                // Finally, insert location data into the database.
                var insertedUri = context.ContentResolver.Insert(
                        WeatherContractOpen.LocationEntryOpen.ContentUri,
                        locationValues
                );

                // The resulting URI contains the ID for the row.  Extract the locationId from the Uri.
                locationId = ContentUris.ParseId(insertedUri);
            }

            location.Close();
            // Wait, that worked?  Yes!
            return locationId;
        }

        public static void ConfigurePeriodicSync (Context context, int syncInterval, int flexTime)
        {
            var account = GetSyncAccount(context);
            var authority = context.GetString(Resource.String.content_authority);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                // we can enable inexact timers in our periodic sync
                //SyncRequest request = new SyncRequest.Builder().
                //        SyncPeriodic(syncInterval, flexTime).
                //        SetSyncAdapter(account, authority).
                //        SetExtras(new Bundle()).Build();
                //ContentResolver.RequestSync(request);
            }
            else
            {
                //ContentResolver.AddPeriodicSync(account,
                //        authority, new Bundle(), syncInterval);
            }
        }

        /**
    * Helper method to have the sync adapter sync immediately
    * @param context The context used to access the account service
    */
        public static void SyncImmediately (Context context)
        {
            var bundle = new Bundle();
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
            var accountManager =
                    (AccountManager)context.GetSystemService(Context.AccountService);

            // Create the account type and default account
            var newAccount = new Account(
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
                OnAccountCreated(newAccount, context);
            }
            return newAccount;
        }

        private static void OnAccountCreated (Account newAccount, Context context)
        {
            /*
             * Since we've created an account
             */
            ConfigurePeriodicSync(context, SyncInterval, SyncFlextime);

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


        private static void SetLocationStatus (Context context, int locationStatus)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutInt(context.GetString(Resource.String.pref_location_status_key), locationStatus);
            prefsEditor.Commit();
        }

        //public void BulkInsertWeather (ArrayList weatherValues, string locationSetting)
        //{
        //    _context.ContentResolver.Delete(WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, null, null);
        //    var insertedCount = 0;
        //    if (weatherValues.Count > 0)
        //    {
        //        var cvArray = new ContentValues[weatherValues.Count];

        //        cvArray = (ContentValues[])weatherValues.ToArray(typeof(ContentValues));

        //        insertedCount = _context.ContentResolver.BulkInsert(WeatherContractOpen.WeatherEntryOpen.CONTENT_URI, cvArray);
        //    }

        //    Log.Debug("Fetch Weather Task", "FetchweatherTask Complete " + insertedCount + " records Inserted");
        //    SetLocationStatus(_context, (int)Helpers.LocationStatus.LocationStatusOk);
        //}

    }
}