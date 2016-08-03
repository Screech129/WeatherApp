using System;
using Android.Widget;
using Android.Content;
using Android.Database;
using Android.Views;
using Com.Bumptech.Glide;

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

        /**
     * Prepare the weather high/lows for presentation.
     */
        private String formatHighLows (double high, double low)
        {
            bool isMetric = Utility.isMetric(context);
            String highLowStr = Utility.formatTemperature(context, high, isMetric) + "/" + Utility.formatTemperature(context, low, isMetric);
            return highLowStr;
        }

        /*
        This is ported from FetchWeatherTask --- but now we go straight from the cursor to the
        string.
     */
        private String convertCursorRowToUXFormat (ICursor cursor)
        {
            // get row indices for our cursor

            String highAndLow = formatHighLows(
                                    cursor.GetDouble(ForecastFragment.COL_WEATHER_MAX_TEMP),
                                    cursor.GetDouble(ForecastFragment.COL_WEATHER_MIN_TEMP));

            return Utility.formatDate(cursor.GetLong(ForecastFragment.COL_WEATHER_DATE)) +
            " - " + cursor.GetString(ForecastFragment.COL_WEATHER_DESC) +
            " - " + highAndLow;
        }

        /*
        Remember that these views are reused as needed.
     */
        public override View NewView (Context context, ICursor cursor, ViewGroup parent)
        {
            // Choose the layout type
            int viewType = GetItemViewType(cursor.Position);
            int layoutId = viewType == 0 ? Resource.Layout.list_item_forecast_today : Resource.Layout.list_item_forecast;
            // TODO: Determine layoutId from viewType
            View view = LayoutInflater.From(context).Inflate(layoutId, parent, false);
            var viewHolder = new MainViewHolder(view);
            view.Tag = viewHolder;
            return view;
        }
        /*
        This is where we fill-in the views with the contents of the cursor.
     */
        public override void BindView (View view, Context context, ICursor cursor)
        {


            // Read weather icon ID from cursor
            int weatherId = cursor.GetInt(ForecastFragment.COL_WEATHER_CONDITION_ID);
            int fallbackIconId;
            // Use placeholder image for now
            if (GetItemViewType(cursor.Position) == 0)
            {

                fallbackIconId = getArtResourceForWeatherCondition(weatherId);
            }
            else
            {
               fallbackIconId  = getIconResourceForWeatherCondition(weatherId);

            }

            Glide.With(context)
                    .Load(Utility.GetArtUrlForWeatherCondition(context, weatherId))
                    .Error(fallbackIconId)
                    .Into(MainViewHolder.iconView);

            // TODO Read date from cursor
            long date = cursor.GetLong(ForecastFragment.COL_WEATHER_DATE);
            MainViewHolder.dateView.Text = Utility.getFriendlyDayString(context, date);


            // TODO Read weather forecast from cursor
            string forecast = cursor.GetString(ForecastFragment.COL_WEATHER_DESC);
            MainViewHolder.descriptionView.Text = forecast;
            // Read user preference for metric or imperial temperature units
            bool isMetric = Utility.isMetric(context);

            // Read high temperature from cursor
            double high = cursor.GetDouble(ForecastFragment.COL_WEATHER_MAX_TEMP);
            MainViewHolder.highTempView.Text = Utility.formatTemperature(context, high, isMetric);

            // TODO Read low temperature from cursor
            double low = cursor.GetDouble(ForecastFragment.COL_WEATHER_MIN_TEMP);
            MainViewHolder.lowTempView.Text = Utility.formatTemperature(context, low, isMetric);
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

