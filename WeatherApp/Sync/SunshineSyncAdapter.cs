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

namespace WeatherApp.Sync
{

    public class SunshineSyncAdapter : AbstractThreadedSyncAdapter
    {
        public const string LOCATION_QUERY_EXTRA = "lqe";
        private Context _context = Application.Context;

        public SunshineSyncAdapter (Context context, bool autoInitialize) :
            base(context, autoInitialize)
        {

        }

        public override void OnPerformSync (Account account, Bundle extras, string authority, ContentProviderClient provider, SyncResult syncResult)
        {
            Log.Debug("SunshineSyncAdapter", "OnPerformSync called.");

            StreamReader reader = null;
            string zipCode = Utility.getPreferredLocation(_context);
            try
            {

                var httpClient = new HttpClient();
                // Construct the URL for the OpenWeatherMap query
                // Possible parameters are available at OWM's forecast API page, at
                // http://openweathermap.org/API#forecast
                Task<string> getJSON = httpClient.GetStringAsync("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + zipCode + ",us&mode=json&units=metric&cnt=7&APPID=83fde89b086ca4abec16cb2a8c245bb8");
                string JSON = getJSON.Result;

                getWeatherDataFromJson(JSON, zipCode);
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

        /**
    * Helper method to have the sync adapter sync immediately
    * @param context The context used to access the account service
    */
        public static void SyncImmediately (Context context)
        {
            Bundle bundle = new Bundle();
            bundle.PutBoolean(ContentResolver.SyncExtrasExpedited, true);
            bundle.PutBoolean(ContentResolver.SyncExtrasManual, true);
            ContentResolver.RequestSync(getSyncAccount(context),
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
        public static Account getSyncAccount (Context context)
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

            }
            return newAccount;
        }

        public void getWeatherDataFromJson (String forecastJsonStr, String locationSetting)
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
                String cityName = cityJson.GetString(OWM_CITY_NAME);

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
                var locationValues = createLocationValues(locationSetting, cityName, lat, lon);
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

        public static ContentValues createLocationValues (string locationSetting, string cityName, double lat, double lon)
        {

            // Create a new map of values, where column names are the keys
            ContentValues locValues = new ContentValues();
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING, locationSetting);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_CITY_NAME, cityName);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LAT, lat);
            locValues.Put(WeatherContractOpen.LocationEntryOpen.COLUMN_COORD_LONG, lon);

            return locValues;
        }


    }
}