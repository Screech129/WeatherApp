
using System;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request.Target;
using Java.Lang;
using Java.Util.Concurrent;

namespace WeatherApp.Widget
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Name = "widget.DetailWidgetRemoteViewsService",Exported = false)]
    //[IntentFilter(new string[] { "WeatherApp.ActionDataUpdated", "android.appwidget.action.APPWIDGET_UPDATE" })]
    public class DetailWidgetRemoteViewsService : RemoteViewsService
    {
        public string LogTag = "DetailWidgetRemoteViewsService";
        public static string[] ForecastColumns =
        {
            WeatherContractOpen.WeatherEntryOpen.TableName + "." + BaseColumns.Id,
            WeatherContractOpen.WeatherEntryOpen.ColumnDate,
            WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId,
            WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc,
            WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp,
            WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp
        };

        // these indices must match the projection
        public const int IndexWeatherId = 0;
        public const int IndexWeatherDate = 1;
        public const int IndexWeatherConditionId = 2;
        public const int IndexWeatherDesc = 3;
        public const int IndexWeatherMaxTemp = 4;
        public const int IndexWeatherMinTemp = 5;


        public override IRemoteViewsFactory OnGetViewFactory (Intent intent)
        {
            return new StackRemoteViewsFactory(this.ApplicationContext,ContentResolver,PackageName,intent);
        }




    }

    class StackRemoteViewsFactory :Java.Lang.Object,RemoteViewsService.IRemoteViewsFactory
    {
        public int Count => data?.Count ?? 0;
        public bool HasStableIds => true;

        public RemoteViews LoadingView => new RemoteViews(packageName, Resource.Layout.widget_detail_list_item);

        public int ViewTypeCount => 1;
        private readonly Context context;
        private ICursor data;
        private readonly string packageName;
        private readonly ContentResolver contentResolver;

        public StackRemoteViewsFactory (Context context, ContentResolver contentResolver, string packageName, Intent intent)
        {
            this.context = context;
            this.contentResolver = contentResolver;           
            this.packageName = packageName;          
        }

        public long GetItemId (int position)
        {
            if (data.MoveToPosition(position))
                return data.GetLong(DetailWidgetRemoteViewsService.IndexWeatherId);
            return position;
        }

        public RemoteViews GetViewAt (int position)
        {
            if (position == AdapterView.InvalidPosition ||
                      data == null || !data.MoveToPosition(position))
            {
                return null;
            }
            var views = new RemoteViews(packageName,
                    Resource.Layout.widget_detail_list_item);
            var weatherId = data.GetInt(DetailWidgetRemoteViewsService.IndexWeatherConditionId);
            var weatherArtResourceId = Utility.GetIconResourceForWeatherCondition(weatherId);
            Bitmap weatherArtImage = null;
            if (!Utility.UsingLocalGraphics(context))
            {
                var weatherArtResourceUrl = Utility.GetArtUrlForWeatherCondition(
                        context, weatherId);
                try
                {
                    weatherArtImage = (Bitmap)Glide.With(context)
                            .Load(weatherArtResourceUrl)

                            .Error(weatherArtResourceId)
                            .Into(Target.SizeOriginal, Target.SizeOriginal).Get();
                }
                catch (System.Exception ex) when (ex is InterruptedException || ex is ExecutionException)
                {

                    Log.Error("DetailWidgetService", "Error retrieving large icon from " + weatherArtResourceUrl, ex);
                }
            }
            var description = data.GetString(DetailWidgetRemoteViewsService.IndexWeatherDesc);
            var dateInMillis = data.GetLong(DetailWidgetRemoteViewsService.IndexWeatherDate);
            var formattedDate = Utility.GetFriendlyDayString(
              context, dateInMillis, false);
            var maxTemp = data.GetDouble(DetailWidgetRemoteViewsService.IndexWeatherMaxTemp);
            var minTemp = data.GetDouble(DetailWidgetRemoteViewsService.IndexWeatherMinTemp);
            var formattedMaxTemperature =
              Utility.FormatTemperature(context, maxTemp, Utility.IsMetric(context));
            var formattedMinTemperature =
                     Utility.FormatTemperature(context, minTemp, Utility.IsMetric(context));
            if (weatherArtImage != null)
            {
                views.SetImageViewBitmap(Resource.Id.widget_icon, weatherArtImage);
            }
            else
            {
                views.SetImageViewResource(Resource.Id.widget_icon, weatherArtResourceId);
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwichMr1)
            {
                SetRemoteContentDescription(views, description);
            }
            views.SetTextViewText(Resource.Id.widget_date, formattedDate);
            views.SetTextViewText(Resource.Id.widget_description, description);
            views.SetTextViewText(Resource.Id.widget_high_temperature, formattedMaxTemperature);
            views.SetTextViewText(Resource.Id.widget_low_temperature, formattedMinTemperature);

            var fillInIntent = new Intent();
            var locationSetting =
                    Utility.GetPreferredLocation(context);
            var weatherUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithDate(
                    locationSetting,
                    dateInMillis);
            fillInIntent.SetData(weatherUri);
            views.SetOnClickFillInIntent(Resource.Id.widget_list_item, fillInIntent);
            return views;
        }

        public void OnCreate ()
        {
        }

        public void OnDataSetChanged ()
        {
            data?.Close();

            // This method is called by the app hosting the widget (e.g., the launcher)
            // However, our ContentProvider is not exported so it doesn't have access to the
            // data. Therefore we need to clear (and finally restore) the calling identity so
            // that calls use our process and permission
            var identityToken = Binder.ClearCallingIdentity();
            var location = Utility.GetPreferredLocation(context);
            var weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen
                    .BuildWeatherLocationWithStartDate(location, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            data = contentResolver.Query(weatherForLocationUri,
                    DetailWidgetRemoteViewsService.ForecastColumns,
                    null,
                    null,
                    WeatherContractOpen.WeatherEntryOpen.ColumnDate + " ASC");
            Binder.RestoreCallingIdentity(identityToken);
        }

        public void OnDestroy ()
        {
            if (data != null)
            {
                data.Close();
                data = null;
            }
        }

        private void SetRemoteContentDescription (RemoteViews views, string description)
        {
            views.SetContentDescription(Resource.Id.widget_icon, description);
        }



    }
}


