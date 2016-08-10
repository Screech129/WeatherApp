using System;
using Android.Widget;
using Android.Content;
using Android.Database;
using Android.Views;
using Com.Bumptech.Glide;
using Android.Util;

namespace WeatherApp
{
    public class ForecastAdapter : CursorAdapter
    {
        public ForecastAdapter (Context context, ICursor c, CursorAdapterFlags flags) :
            base(context, c, flags)
        {
        }

        Context context = Android.App.Application.Context;
        private const int VIEW_TYPE_TODAY = 0;
        private const int VIEW_TYPE_FUTURE_DAY = 1;
        private const int VIEW_TYPE_COUNT = 2;
        private bool _useTodayLayout;
        public void setUseTodayLayout (bool useTodayLayout)
        {
            _useTodayLayout = useTodayLayout;
        }

        public override int GetItemViewType (int position)
        {
            return (position == 0 && _useTodayLayout) ? VIEW_TYPE_TODAY : VIEW_TYPE_FUTURE_DAY;
        }

        public override int ViewTypeCount
        {
            get
            {
                return 2;
            }
        }
        /*
        Remember that these views are reused as needed.
     */
        public override View NewView (Context context, ICursor cursor, ViewGroup parent)
        {
            // Choose the layout type
            int viewType = GetItemViewType(cursor.Position);
            int layoutId = viewType == 0 ? Resource.Layout.list_item_forecast_today : Resource.Layout.list_item_forecast;

            View view = LayoutInflater.From(context).Inflate(layoutId, parent, false);
            MainViewHolder viewHolder = view.Tag as MainViewHolder;
            if (viewHolder == null)
            {
                viewHolder = new MainViewHolder();
                viewHolder.iconView = (ImageView)view.FindViewById(Resource.Id.list_item_icon);
                viewHolder.dateView = (TextView)view.FindViewById(Resource.Id.list_item_date_textview);
                viewHolder.descriptionView = (TextView)view.FindViewById(Resource.Id.list_item_forecast_textview);
                viewHolder.highTempView = (TextView)view.FindViewById(Resource.Id.list_item_high_textview);
                viewHolder.lowTempView = (TextView)view.FindViewById(Resource.Id.list_item_low_textview);
                view.Tag = viewHolder;
            }
            return view;
        }
        /*
        This is where we fill-in the views with the contents of the cursor.
     */
        public override void BindView (View view, Context context, ICursor cursor)
        {

            var viewHolder = (MainViewHolder)view.Tag;

            int weatherId = cursor.GetInt(ForecastFragment.COL_WEATHER_CONDITION_ID);
            int fallbackIconId;

            int viewType = GetItemViewType(Cursor.Position);

            if (viewType == 0)
            {

                fallbackIconId = getArtResourceForWeatherCondition(weatherId);
            }
            else
            {
                fallbackIconId = getIconResourceForWeatherCondition(weatherId);

            }

            Glide.With(context)
                    .Load(Utility.GetArtUrlForWeatherCondition(context, weatherId))
                    .Error(fallbackIconId)
                    .Into(viewHolder.iconView);

            // TODO Read date from cursor
            long date = cursor.GetLong(ForecastFragment.COL_WEATHER_DATE);
            viewHolder.dateView.Text = Utility.GetFriendlyDayString(context, date);


            // TODO Read weather forecast from cursor
            string forecast = cursor.GetString(ForecastFragment.COL_WEATHER_DESC);
            viewHolder.descriptionView.Text = forecast;
            // Read user preference for metric or imperial temperature units
            bool isMetric = Utility.IsMetric(context);

            // Read high temperature from cursor
            double high = cursor.GetDouble(ForecastFragment.COL_WEATHER_MAX_TEMP);
            viewHolder.highTempView.Text = Utility.FormatTemperature(context, high, isMetric);

            // TODO Read low temperature from cursor
            double low = cursor.GetDouble(ForecastFragment.COL_WEATHER_MIN_TEMP);
            viewHolder.lowTempView.Text = Utility.FormatTemperature(context, low, isMetric);
        }

        /**
 * Helper method to provide the icon resource id according to the weather condition id returned
 * by the OpenWeatherMap call.
 * @param weatherId from OpenWeatherMap API response
 * @return resource id for the corresponding icon. -1 if no relation is found.
 */
        public static int getIconResourceForWeatherCondition (int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            if (weatherId >= 200 && weatherId <= 232)
            {
                return Resource.Drawable.ic_storm;
            }
            else if (weatherId >= 300 && weatherId <= 321)
            {
                return Resource.Drawable.ic_light_rain;
            }
            else if (weatherId >= 500 && weatherId <= 504)
            {
                return Resource.Drawable.ic_rain;
            }
            else if (weatherId == 511)
            {
                return Resource.Drawable.ic_snow;
            }
            else if (weatherId >= 520 && weatherId <= 531)
            {
                return Resource.Drawable.ic_rain;
            }
            else if (weatherId >= 600 && weatherId <= 622)
            {
                return Resource.Drawable.ic_snow;
            }
            else if (weatherId >= 701 && weatherId <= 761)
            {
                return Resource.Drawable.ic_fog;
            }
            else if (weatherId == 761 || weatherId == 781)
            {
                return Resource.Drawable.ic_storm;
            }
            else if (weatherId == 800)
            {
                return Resource.Drawable.ic_clear;
            }
            else if (weatherId == 801)
            {
                return Resource.Drawable.ic_light_clouds;
            }
            else if (weatherId >= 802 && weatherId <= 804)
            {
                return Resource.Drawable.ic_cloudy;
            }
            return -1;
        }

        /**
 * Helper method to provide the art resource id according to the weather condition id returned
 * by the OpenWeatherMap call.
 * @param weatherId from OpenWeatherMap API response
 * @return resource id for the corresponding image. -1 if no relation is found.
 */
        public static int getArtResourceForWeatherCondition (int weatherId)
        {
            // Based on weather code data found at:
            // http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes
            if (weatherId >= 200 && weatherId <= 232)
            {
                return Resource.Drawable.art_storm;
            }
            else if (weatherId >= 300 && weatherId <= 321)
            {
                return Resource.Drawable.art_light_rain;
            }
            else if (weatherId >= 500 && weatherId <= 504)
            {
                return Resource.Drawable.art_rain;
            }
            else if (weatherId == 511)
            {
                return Resource.Drawable.art_snow;
            }
            else if (weatherId >= 520 && weatherId <= 531)
            {
                return Resource.Drawable.art_rain;
            }
            else if (weatherId >= 600 && weatherId <= 622)
            {
                return Resource.Drawable.art_rain;
            }
            else if (weatherId >= 701 && weatherId <= 761)
            {
                return Resource.Drawable.art_fog;
            }
            else if (weatherId == 761 || weatherId == 781)
            {
                return Resource.Drawable.art_storm;
            }
            else if (weatherId == 800)
            {
                return Resource.Drawable.art_clear;
            }
            else if (weatherId == 801)
            {
                return Resource.Drawable.art_light_clouds;
            }
            else if (weatherId >= 802 && weatherId <= 804)
            {
                return Resource.Drawable.art_clouds;
            }
            return -1;
        }

    }

}

