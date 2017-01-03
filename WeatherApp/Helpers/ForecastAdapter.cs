using System;
using Android.Widget;
using Android.Content;
using Android.Database;
using Android.Views;
using Com.Bumptech.Glide;
using Android.Util;
using Android.Support.V7.Widget;
namespace WeatherApp
{
    public class ForecastAdapter : RecyclerView.Adapter

    {
        private const int VIEW_TYPE_TODAY = 0;
        private const int VIEW_TYPE_FUTURE_DAY = 1;
        private const int VIEW_TYPE_COUNT = 2;
        private bool _useTodayLayout;
        private Context context;
        public static ICursor cursor;
        public event EventHandler<long> ItemClick; 
        public static View EmptyView;
        public class ForecastAdapterViewHolder : RecyclerView.ViewHolder
        {
            public ImageView iconView;
            public TextView dateView;
            public TextView descriptionView;
            public TextView highTempView;
            public TextView lowTempView;
            public ForecastAdapterViewHolder (View view, Action<ForecastAdapterViewHolder,int> listener) : base(view)
            {

                iconView = (ImageView)view.FindViewById(Resource.Id.list_item_icon);
                dateView = (TextView)view.FindViewById(Resource.Id.list_item_date_textview);
                descriptionView = (TextView)view.FindViewById(Resource.Id.list_item_forecast_textview);
                highTempView = (TextView)view.FindViewById(Resource.Id.list_item_high_textview);
                lowTempView = (TextView)view.FindViewById(Resource.Id.list_item_low_textview);
                view.Click += (sender, e) => listener(this,AdapterPosition);
            }

           
        }

       
        public ForecastAdapter (Context context, View emptyView)
        {
            this.context = context;
            EmptyView = emptyView;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            if (parent.GetType() == typeof(RecyclerView))
            {
                int layoutId = -1;
                switch (viewType)
                {
                    case VIEW_TYPE_TODAY:
                        {
                            layoutId = Resource.Layout.list_item_forecast_today;
                            break;
                        }
                    case VIEW_TYPE_FUTURE_DAY:
                        {
                            layoutId = Resource.Layout.list_item_forecast;
                            break;
                        }
                }
                View view = LayoutInflater.From(parent.Context).Inflate(layoutId, parent, false);
                view.Focusable = true;
                return new ForecastAdapterViewHolder(view, OnClick);
            }
            else
            {
                throw new AndroidRuntimeException("Not bound to RecyclerView");
            }
        }

        public override void OnBindViewHolder (RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = (ForecastAdapterViewHolder)viewHolder;
            cursor.MoveToPosition(position);
            int weatherId = cursor.GetInt(ForecastFragment.COL_WEATHER_CONDITION_ID);
            int defaultImage;

            switch (GetItemViewType(position))
            {
                case VIEW_TYPE_TODAY:
                    defaultImage = Utility.GetArtResourceForWeatherCondition(weatherId);
                    break;
                default:
                    defaultImage = Utility.GetIconResourceForWeatherCondition(weatherId);
                    break;
            }

            if (Utility.UsingLocalGraphics(context))
            {
                holder.iconView.SetImageResource(defaultImage);
            }
            else
            {
                Glide.With(context)
                        .Load(Utility.GetArtUrlForWeatherCondition(context, weatherId))
                        .Error(defaultImage)                       
                        .Into(holder.iconView);
            }

            // Read date from cursor
            long dateInMillis = cursor.GetLong(ForecastFragment.COL_WEATHER_DATE);

            // Find TextView and set formatted date on it
            holder.dateView.Text = Utility.GetFriendlyDayString(context, dateInMillis);

            // Read weather forecast from cursor
            String description = Utility.GetStringForWeatherCondition(context, weatherId);

            // Find TextView and set weather forecast on it
            holder.descriptionView.Text=description;
            holder.descriptionView.ContentDescription=context.GetString(Resource.String.a11y_forecast, description);

            // For accessibility, we don't want a content description for the icon field
            // because the information is repeated in the description view and the icon
            // is not individually selectable

            // Read high temperature from cursor
            double high = cursor.GetDouble(ForecastFragment.COL_WEATHER_MAX_TEMP);
            String highString = Utility.FormatTemperature(context, high,Utility.IsMetric(context));
            holder.highTempView.Text = highString;
            holder.highTempView.ContentDescription = context.GetString(Resource.String.a11y_high_temp, highString);

            // Read low temperature from cursor
            double low = cursor.GetDouble(ForecastFragment.COL_WEATHER_MIN_TEMP);
            String lowString = Utility.FormatTemperature(context, low, Utility.IsMetric(context));
            holder.lowTempView.Text=lowString;
            holder.lowTempView.ContentDescription = context.GetString(Resource.String.a11y_low_temp, lowString);
        }

        
        public void SetUseTodayLayout (bool useTodayLayout)
        {
            _useTodayLayout = useTodayLayout;
        }

        public override int GetItemViewType (int position)
        {
            return (position == 0 && _useTodayLayout) ? VIEW_TYPE_TODAY : VIEW_TYPE_FUTURE_DAY;
        }

        public override int ItemCount
        {
            get
            {
                if (null == cursor) return 0;
                return cursor.Count;
            }
        }
        public void OnClick (ForecastAdapterViewHolder view, int position)
        {
            int adapterPosition = position;
            cursor.MoveToPosition(adapterPosition);
            int dateColumnIndex = cursor.GetColumnIndex(WeatherContractOpen.WeatherEntryOpen.COLUMN_DATE);
            ItemClick?.Invoke(view,cursor.GetLong(dateColumnIndex));
        }
        public void SwapCursor (ICursor newCursor)
        {
            cursor = newCursor;
            NotifyDataSetChanged();
            EmptyView.Visibility = (ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone);
        }

        public ICursor GetCursor ()
        {
            return cursor;
        }
    }

}

