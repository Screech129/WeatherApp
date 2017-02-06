using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WeatherApp.Widget
{
    [Service]
    [IntentFilter(new string[] { "WeatherApp.ActionDataUpdated", "android.appwidget.action.APPWIDGET_UPDATE" })]

    public class TodayWidgetIntentService : IntentService
    {
        private static readonly string[] ForecastColumns = {
            WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId,
            WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc,
            WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp,
            WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp
    };
        // these indices must match the projection
        private const int IndexWeatherId = 0;
        private const int IndexShortDesc = 1;
        private const int IndexMaxTemp = 2;
        private const int IndexMinTemp = 2;

        public TodayWidgetIntentService ()
        {

        }


        protected override void OnHandleIntent (Intent intent)
        {
            // Retrieve all of the Today widget ids: these are the widgets we need to update
            var appWidgetManager = AppWidgetManager.GetInstance(this);
            var componentName = new ComponentName(this,Java.Lang.Class.FromType(typeof(TodayWidgetProvider)).Name);
            var appWidgetIds = appWidgetManager.GetAppWidgetIds(componentName);
            
            // Get today's data from the ContentProvider
            var location = Utility.GetPreferredLocation(this);
            var weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithStartDate(
                location, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            var data = ContentResolver.Query(weatherForLocationUri, ForecastColumns, null,
                    null, WeatherContractOpen.WeatherEntryOpen.ColumnDate + " ASC");
            if (data == null)
            {
                return;
            }
            if (!data.MoveToFirst())
            {
                data.Close();
                return;
            }

            // Extract the weather data from the Cursor
            var weatherId = data.GetInt(IndexWeatherId);
            var weatherArtResourceId = Utility.GetArtResourceForWeatherCondition(weatherId);
            var description = data.GetString(IndexShortDesc);
            var maxTemp = data.GetDouble(IndexMaxTemp);
            var minTemp = data.GetDouble(IndexMinTemp);
            var formattedMaxTemperature = Utility.FormatTemperature(this, maxTemp, Utility.IsMetric(this));
            var formattedMinTemperature = Utility.FormatTemperature(this, minTemp, Utility.IsMetric(this));
            data.Close();

            // Perform this loop procedure for each Today widget
            foreach (var appWidgetId in appWidgetIds)
            {
                // Find the correct layout based on the widget's width
                var widgetWidth = GetWidgetWidth(appWidgetManager, appWidgetId);
                var defaultWidth = Resources.GetDimensionPixelSize(Resource.Dimension.widget_today_default_width);
                var largeWidth = Resources.GetDimensionPixelSize(Resource.Dimension.widget_today_large_width);
                int layoutId;
                if (widgetWidth >= largeWidth)
                {
                    layoutId = Resource.Layout.widget_today_large;
                }
                else if (widgetWidth >= defaultWidth)
                {
                    layoutId = Resource.Layout.widget_today;
                }
                else
                {
                    layoutId = Resource.Layout.widget_today_small;
                }

                var views = new RemoteViews(PackageName, layoutId);

                // Add the data to the RemoteViews
                views.SetImageViewResource(Resource.Id.widget_icon, weatherArtResourceId);
                // Content Descriptions for RemoteViews were only added in ICS MR1
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwichMr1)
                {
                    SetRemoteContentDescription(views, description);
                }
                views.SetTextViewText(Resource.Id.widget_description, description);
                views.SetTextViewText(Resource.Id.widget_high_temperature, formattedMaxTemperature);
                views.SetTextViewText(Resource.Id.widget_low_temperature, formattedMinTemperature);
                // Create an Intent to launch MainActivity
                var launchIntent = new Intent(this, typeof(MainActivity));
                var pendingIntent = PendingIntent.GetActivity(this, 0, launchIntent, 0);
                views.SetOnClickPendingIntent(Resource.Id.widget, pendingIntent);

                // Tell the AppWidgetManager to perform an update on the current app widget
                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }

        private int GetWidgetWidth (AppWidgetManager appWidgetManager, int appWidgetId)
        {
            // Prior to Jelly Bean, widgets were always their default size
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
            {
                return Resources.GetDimensionPixelSize(Resource.Dimension.widget_today_default_width);
            }
            // For Jelly Bean and higher devices, widgets can be resized - the current size can be
            // retrieved from the newly added App Widget Options
            return GetWidgetWidthFromOptions(appWidgetManager, appWidgetId);
        }

    private int GetWidgetWidthFromOptions (AppWidgetManager appWidgetManager, int appWidgetId)
        {
            var options = appWidgetManager.GetAppWidgetOptions(appWidgetId);
            if (options.ContainsKey(AppWidgetManager.OptionAppwidgetMinWidth))
            {
                var minWidthDp = options.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
                // The width returned is in dp, but we'll convert it to pixels to match the other widths
                var displayMetrics = Resources.DisplayMetrics;
                return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, minWidthDp,
                        displayMetrics);
            }
            return Resources.GetDimensionPixelSize(Resource.Dimension.widget_today_default_width);
        }

        private void SetRemoteContentDescription (RemoteViews views, String description)
        {
            views.SetContentDescription(Resource.Id.widget_icon, description);
        }
    }
}