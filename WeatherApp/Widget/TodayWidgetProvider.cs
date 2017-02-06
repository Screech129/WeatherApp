

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using WeatherApp.Sync;

namespace WeatherApp.Widget
{
    [BroadcastReceiver(Label = "@string/title_widget_today", Name = "android.appwidget.provider")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", "WeatherApp.ActionDataUpdated" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget_info_today")]
    public class TodayWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate (Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var intent = new Intent(context, typeof(TodayWidgetIntentService));
            context.StartService(intent);
        }

        public override void OnAppWidgetOptionsChanged (Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            var intent = new Intent(context, typeof(TodayWidgetIntentService));
            context.StartService(intent);

        }

        public override void OnReceive (Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            if (SunshineSyncAdapter.ActionDataUpdated == intent.Action)
            {
                context.StartService(new Intent(context, typeof(TodayWidgetIntentService)));
            }
        }
    }
}