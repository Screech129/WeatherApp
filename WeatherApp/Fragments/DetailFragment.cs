using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Database;
using Com.Bumptech.Glide;
using Android.Support.V4;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Java.Lang;
using Android.Support.V7.App;

namespace WeatherApp
{
    public class DetailFragment : Android.Support.V4.App.Fragment, Android.Support.V4.App.LoaderManager.ILoaderCallbacks
    {


        string forecastString = "";
        private const int URL_LOADER = 0;
        public const string DETAIL_URI = "URI";
        private Android.Net.Uri globalUri;
        private ShareActionProvider shareActionProvider;




        private string[] DETAIL_COLUMNS = {
			// In this case the id needs to be fully qualified with a table name, since
			// the content provider joins the location & weather tables in the background
			// (both have an _id column)
			// On the one hand, that's annoying.  On the other, you can search the weather table
			// using the location set by the user, which is only in the Location table.
			// So the convenience is worth it.
			WeatherContractOpen.WeatherEntryOpen.TABLE_NAME + "." + WeatherContractOpen.WeatherEntryOpen._ID,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_SHORT_DESC,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MAX_TEMP,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_MIN_TEMP,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_HUMIDITY,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_PRESSURE,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_WIND_SPEED,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_DEGREES,
            WeatherContractOpen.WeatherEntryOpen.COLUMN_WEATHER_id,
            WeatherContractOpen.LocationEntryOpen.COLUMN_LOCATION_SETTING

        };

        // These indices are tied to FORECAST_COLUMNS.  If FORECAST_COLUMNS changes, these
        // must change.
        public const int COL_WEATHER_ID = 0;
        public const int COL_WEATHER_DATE = 1;
        public const int COL_WEATHER_DESC = 2;
        public const int COL_WEATHER_MAX_TEMP = 3;
        public const int COL_WEATHER_MIN_TEMP = 4;
        public const int COL_WEATHER_HUMIDITY = 5;
        public const int COL_WEATHER_PRESSURE = 6;
        public const int COL_WEATHER_WIND_SPEED = 7;
        public const int COL_WEATHER_DEGREES = 8;
        public const int COL_WEATHER_CONDITION_ID = 9;

        ImageView iconView;
        TextView dateView;
        TextView descriptionView;
        TextView highTempView;
        TextView lowTempView;
        TextView humidityView;
        TextView humidityLabelView;
        TextView windView;
        TextView windLabelView;
        TextView pressureView;
        TextView pressureLabelView;

        public DetailFragment ()
        {
            HasOptionsMenu = true;
        }
        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Bundle arguments = Arguments;
            if (arguments != null)
            {
                globalUri = (Android.Net.Uri)arguments.GetParcelable(DetailFragment.DETAIL_URI);
            }

            View rootView = inflater.Inflate(Resource.Layout.fragment_detail, container, false);
            iconView = rootView.FindViewById<ImageView>(Resource.Id.detail_icon);
            dateView = rootView.FindViewById<TextView>(Resource.Id.detail_date_textview);
            descriptionView = rootView.FindViewById<TextView>(Resource.Id.detail_forecast_textview);
            highTempView = rootView.FindViewById<TextView>(Resource.Id.detail_high_textview);
            lowTempView = rootView.FindViewById<TextView>(Resource.Id.detail_low_textview);
            humidityView = rootView.FindViewById<TextView>(Resource.Id.detail_humidity_textview);
            humidityLabelView = rootView.FindViewById<TextView>(Resource.Id.detail_humidity_label_textview);
            windView = rootView.FindViewById<TextView>(Resource.Id.detail_wind_textview);
            windLabelView = rootView.FindViewById<TextView>(Resource.Id.detail_wind_label_textview);
            pressureView = rootView.FindViewById<TextView>(Resource.Id.detail_pressure_textview);
            pressureLabelView = rootView.FindViewById<TextView>(Resource.Id.detail_pressure_label_textview);

            return rootView;
        }

        private void FinishCreatingMenu (IMenu menu)
        {
            IMenuItem menuItem = menu.FindItem(Resource.Id.action_share);
            menuItem.SetIntent(CreateShareForecastIntent());
        }
        public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
        {
            if (Activity.GetType().Equals(typeof(DetailActivity)))
            {
                inflater.Inflate(Resource.Menu.detail_fragment, menu);
                FinishCreatingMenu(menu);
            }
        }

        private Intent CreateShareForecastIntent ()
        {
            Intent shareIntent = new Intent(Intent.ActionSend);
            shareIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            shareIntent.SetType("text/plain");
            shareIntent.PutExtra(Intent.ExtraText, forecastString + " #SunshineApp");
            return shareIntent;
        }

        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            LoaderManager.InitLoader(URL_LOADER, null, this);
            base.OnActivityCreated(savedInstanceState);
        }
        public void OnLocationChanged (string newLocation)
        {
            Android.Net.Uri uri = globalUri;
            if (uri != null)
            {
                long date = WeatherContractOpen.WeatherEntryOpen.getDateFromUri(uri);
                Android.Net.Uri updatedUri = WeatherContractOpen.WeatherEntryOpen.buildWeatherLocationWithDate(newLocation, date);
                globalUri = updatedUri;
                LoaderManager.RestartLoader(URL_LOADER, null, this);
            }
        }
        public Android.Support.V4.Content.Loader OnCreateLoader (int id, Bundle args)
        {

            if (globalUri != null)
            {

                return new Android.Support.V4.Content.CursorLoader(Activity, globalUri, DETAIL_COLUMNS, null, null, null);
            }
            return null;
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {

            if (item.ItemId == Android.Resource.Id.Home)
            {
                Activity.Finish();
                return true;
            }
               
            return true;

        }

        public void OnLoadFinished (Android.Support.V4.Content.Loader loader, Java.Lang.Object data)
        {
            var cursor = (ICursor)data;
            if (cursor != null && cursor.MoveToNext())
            {
                int weatherId = cursor.GetInt(COL_WEATHER_CONDITION_ID);

                if (Utility.UsingLocalGraphics(Activity))
                {
                    iconView.SetImageResource(Utility.GetArtResourceForWeatherCondition(weatherId));
                }
                else
                {
                    // Use weather art image
                    Glide.With(this)
                            .Load(Utility.GetArtUrlForWeatherCondition(Activity, weatherId))
                            .Error(Utility.GetArtResourceForWeatherCondition(weatherId))
                            .Into(iconView);
                }

                var date = cursor.GetLong(COL_WEATHER_DATE);
                dateView.Text = Utility.GetFullFriendlyDayString(Activity, date);

                // Get description from weather condition ID
                string description = Utility.GetStringForWeatherCondition(Activity, weatherId);
                descriptionView.Text = description;
                descriptionView.ContentDescription = (GetString(Resource.String.a11y_forecast, description));

                // For accessibility, add a content description to the icon field. Because the ImageView
                // is independently focusable, it's better to have a description of the image. Using
                // null is appropriate when the image is purely decorative or when the image already
                // has text describing it in the same UI component.
                iconView.ContentDescription = (GetString(Resource.String.a11y_forecast_icon, description));

                var isMetric = Utility.IsMetric(Activity);

                var high = cursor.GetDouble(COL_WEATHER_MAX_TEMP);
                highTempView.Text = Utility.FormatTemperature(Activity, high, isMetric);
                highTempView.ContentDescription = (GetString(Resource.String.a11y_high_temp, Utility.FormatTemperature(Activity, high, isMetric)));

                var low = cursor.GetDouble(COL_WEATHER_MIN_TEMP);
                lowTempView.Text = Utility.FormatTemperature(Activity, low, isMetric);
                lowTempView.ContentDescription = (GetString(Resource.String.a11y_low_temp, Utility.FormatTemperature(Activity, low, isMetric)));


                var humidity = cursor.GetLong(COL_WEATHER_HUMIDITY);
                humidityView.Text = string.Format(Activity.GetString(Resource.String.format_humidity), humidity);
                humidityView.ContentDescription = (GetString(Resource.String.a11y_humidity, humidityView.Text));
                humidityLabelView.ContentDescription = (humidityView.ContentDescription);

                var wind = cursor.GetLong(COL_WEATHER_WIND_SPEED);
                windView.Text = Utility.GetFormattedWind(Activity, wind, cursor.GetLong(COL_WEATHER_DEGREES));
                windView.ContentDescription = (GetString(Resource.String.a11y_wind, windView.Text));
                windLabelView.ContentDescription = (windView.ContentDescription);

                var pressure = cursor.GetLong(COL_WEATHER_PRESSURE);
                pressureView.Text = string.Format(Activity.GetString(Resource.String.format_pressure), pressure);
                pressureView.ContentDescription = (GetString(Resource.String.a11y_pressure, pressureView.Text));
                pressureLabelView.ContentDescription = (pressureView.ContentDescription);

                forecastString = convertCursorRowToUXFormat(cursor);

                if (shareActionProvider != null)
                {
                    shareActionProvider.SetShareIntent(CreateShareForecastIntent());
                }
            }

            var activity = (AppCompatActivity)Activity;
            var toolbarView = (Android.Support.V7.Widget.Toolbar)View.FindViewById(Resource.Id.toolbar);

            if (Activity.GetType() == typeof(DetailActivity))
            {
                activity.SupportStartPostponedEnterTransition();

                if (null != toolbarView)
                {
                    activity.SetSupportActionBar(toolbarView);

                    activity.SupportActionBar.SetDisplayShowTitleEnabled(false);
                    activity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                }
            }
            else
            {
                if (null != toolbarView)
                {
                    IMenu menu = toolbarView.Menu;
                    if (null != menu) menu.Clear();
                    toolbarView.InflateMenu(Resource.Menu.detail_fragment);
                    FinishCreatingMenu(toolbarView.Menu);
                }
            }
        }

        public void OnLoaderReset (Android.Support.V4.Content.Loader loader)
        {

        }

        private string formatHighLows (double high, double low)
        {
            bool isMetric = Utility.IsMetric(Activity);
            string highLowStr = Utility.FormatTemperature(Activity, high, isMetric) + "/" + Utility.FormatTemperature(Activity, low, isMetric);
            return highLowStr;
        }

        /*
        This is ported from FetchWeatherTask --- but now we go straight from the cursor to the
        string.
     */
        private string convertCursorRowToUXFormat (ICursor cursor)
        {
            // get row indices for our cursor

            string highAndLow = formatHighLows(
                                    cursor.GetDouble(COL_WEATHER_MAX_TEMP),
                                    cursor.GetDouble(COL_WEATHER_MIN_TEMP));

            return Utility.FormatDate(cursor.GetLong(COL_WEATHER_DATE)) +
            " - " + cursor.GetString(COL_WEATHER_DESC) +
            " - " + highAndLow;
        }
    }



}




