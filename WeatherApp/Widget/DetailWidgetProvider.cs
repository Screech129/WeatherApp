

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Widget;
using Java.Lang;
using WeatherApp.Sync;

namespace WeatherApp.Widget
{
    [BroadcastReceiver(Label = "@string/title_widget_detail", Name = "widget.DetailWidgetProvider")]
    [IntentFilter(new string[] {"android.appwidget.action.APPWIDGET_UPDATE", "WeatherApp.ActionDataUpdated"})]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget_info_detail")]
    public class DetailWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            // Perform this loop procedure for each App Widget that belongs to this provider
            foreach (var appWidgetId in appWidgetIds)
            {
                var views = new RemoteViews(context.PackageName, Resource.Layout.widget_detail);

                // Create an Intent to launch MainActivity
                var intent = new Intent(context, typeof(MainActivity));
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, 0);
                views.SetOnClickPendingIntent(Resource.Id.widget, pendingIntent);

                // Set up the collection
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                {
                    SetRemoteAdapter(context, views);
                }
                else
                {
                   SetRemoteAdapterV11(context, views);
                }
                var useDetailActivity = context.Resources.GetBoolean(Resource.Boolean.use_detail_activity);
                var clickIntentTemplate = useDetailActivity
                    ? new Intent(context, typeof(DetailActivity))
                    : new Intent(context, typeof(MainActivity));
                var clickPendingIntentTemplate = TaskStackBuilder.Create(context)
                    .AddNextIntentWithParentStack(clickIntentTemplate)
                    .GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
                views.SetPendingIntentTemplate(Resource.Id.widget_list, clickPendingIntentTemplate);
                views.SetEmptyView(Resource.Id.widget_list, Resource.Id.widget_empty);

                // Tell the AppWidgetManager to perform an update on the current app widget
                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }

        public override void OnReceive(Context context, Intent intent)

        {
            base.OnReceive(context, intent);
            if (SunshineSyncAdapter.ActionDataUpdated.Equals(intent.Action))
            {
                var appWidgetManager = AppWidgetManager.GetInstance(context);
                var appWidgetIds = appWidgetManager.GetAppWidgetIds(
                    new ComponentName(context, Class));
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.widget_list);
            }
        }


        /**
         * Sets the remote adapter used to fill in the list items
         *
         * @param views RemoteViews to set the RemoteAdapter
         */
        
        private void SetRemoteAdapter(Context context, RemoteViews views)
        {
            views.SetRemoteAdapter(Resource.Id.widget_list,
                new Intent(context, typeof(DetailWidgetRemoteViewsService)));
        }

        /**
         * Sets the remote adapter used to fill in the list items
         *
         * @param views RemoteViews to set the RemoteAdapter
         */
       

        private void SetRemoteAdapterV11(Context context,RemoteViews views)
        {
          
            views.SetRemoteAdapter(0, Resource.Id.widget_list,
                new Intent(context, typeof(DetailWidgetRemoteViewsService)))
            ;
        }

    }
}