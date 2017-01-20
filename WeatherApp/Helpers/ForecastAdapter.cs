using System;
using Android.Widget;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Com.Bumptech.Glide;
using Android.Util;
using Android.Support.V7.Widget;
using WeatherApp.Fragments;

namespace WeatherApp
{
    public class ForecastAdapter : RecyclerView.Adapter

    {
        private const int ViewTypeToday = 0;
        private const int ViewTypeFutureDay = 1;
        private const int ViewTypeCount = 2;
        private bool useTodayLayout;
        private Context context;
        public static ICursor Cursor;
        public event EventHandler<long> ItemClick; 
        public static View EmptyView;
        private ItemChoiceManager itemChoiceManager;
        public class ForecastAdapterViewHolder : RecyclerView.ViewHolder
        {
            public ImageView IconView;
            public TextView DateView;
            public TextView DescriptionView;
            public TextView HighTempView;
            public TextView LowTempView;
            public ForecastAdapterViewHolder (View view, Action<ForecastAdapterViewHolder,int> listener) : base(view)
            {

                IconView = (ImageView)view.FindViewById(Resource.Id.list_item_icon);
                DateView = (TextView)view.FindViewById(Resource.Id.list_item_date_textview);
                DescriptionView = (TextView)view.FindViewById(Resource.Id.list_item_forecast_textview);
                HighTempView = (TextView)view.FindViewById(Resource.Id.list_item_high_textview);
                LowTempView = (TextView)view.FindViewById(Resource.Id.list_item_low_textview);
                view.Click += (sender, e) => listener(this,AdapterPosition);

            }

           
        }

       
        public ForecastAdapter (Context context, View emptyView, int choiceMode)
        {
            this.context = context;
            EmptyView = emptyView;
            itemChoiceManager = new ItemChoiceManager(context,this);
            itemChoiceManager.SetChoiceMode(choiceMode);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            if (parent.GetType() == typeof(RecyclerView))
            {
                var layoutId = -1;
                switch (viewType)
                {
                    case ViewTypeToday:
                        {
                            layoutId = Resource.Layout.list_item_forecast_today;
                            break;
                        }
                    case ViewTypeFutureDay:
                        {
                            layoutId = Resource.Layout.list_item_forecast;
                            break;
                        }
                }
                var view = LayoutInflater.From(parent.Context).Inflate(layoutId, parent, false);
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
            Cursor.MoveToPosition(position);
            var weatherId = Cursor.GetInt(ForecastFragment.ColWeatherConditionId);
            int defaultImage;

            switch (GetItemViewType(position))
            {
                case ViewTypeToday:
                    defaultImage = Utility.GetArtResourceForWeatherCondition(weatherId);
                    break;
                default:
                    defaultImage = Utility.GetIconResourceForWeatherCondition(weatherId);
                    break;
            }

            if (Utility.UsingLocalGraphics(context))
            {
                holder.IconView.SetImageResource(defaultImage);
            }
            else
            {
                Glide.With(context)
                        .Load(Utility.GetArtUrlForWeatherCondition(context, weatherId))
                        .Error(defaultImage)                       
                        .Into(holder.IconView);
            }

            ViewCompat.SetTransitionName(holder.IconView, "iconView" + position);


            // Read date from cursor
            var dateInMillis = Cursor.GetLong(ForecastFragment.ColWeatherDate);

            // Find TextView and set formatted date on it
            holder.DateView.Text = Utility.GetFriendlyDayString(context, dateInMillis);

            // Read weather forecast from cursor
            var description = Utility.GetStringForWeatherCondition(context, weatherId);

            // Find TextView and set weather forecast on it
            holder.DescriptionView.Text=description;
            holder.DescriptionView.ContentDescription=context.GetString(Resource.String.a11y_forecast, description);

            // For accessibility, we don't want a content description for the icon field
            // because the information is repeated in the description view and the icon
            // is not individually selectable

            // Read high temperature from cursor
            var high = Cursor.GetDouble(ForecastFragment.ColWeatherMaxTemp);
            var highString = Utility.FormatTemperature(context, high,Utility.IsMetric(context));
            holder.HighTempView.Text = highString;
            holder.HighTempView.ContentDescription = context.GetString(Resource.String.a11y_high_temp, highString);

            // Read low temperature from cursor
            var low = Cursor.GetDouble(ForecastFragment.ColWeatherMinTemp);
            var lowString = Utility.FormatTemperature(context, low, Utility.IsMetric(context));
            holder.LowTempView.Text=lowString;
            holder.LowTempView.ContentDescription = context.GetString(Resource.String.a11y_low_temp, lowString);

            itemChoiceManager.OnBindViewHolder(viewHolder,position);
        }

        public void OnRestoreInstanceState (Bundle savedInstanceState)
        {
            itemChoiceManager.OnRestoreInstanceState(savedInstanceState);
        }

        public void OnSaveInstanceState (Bundle outState)
        {
            itemChoiceManager.OnSaveInstanceState(outState);
        }

        public int GetSelectedItemPosition()
        {
            return itemChoiceManager.GetSelectedItemPosition();
        }

        public void SetUseTodayLayout (bool useTodayLayout)
        {
            this.useTodayLayout = useTodayLayout;
        }

        public override int GetItemViewType (int position)
        {
            return (position == 0 && useTodayLayout) ? ViewTypeToday : ViewTypeFutureDay;
        }

        public override int ItemCount
        {
            get
            {
                if (null == Cursor) return 0;
                return Cursor.Count;
            }
        }
        public void OnClick (ForecastAdapterViewHolder view, int position)
        {
            var adapterPosition = position;
            Cursor.MoveToPosition(adapterPosition);
            var dateColumnIndex = Cursor.GetColumnIndex(WeatherContractOpen.WeatherEntryOpen.ColumnDate);
            ItemClick?.Invoke(view,Cursor.GetLong(dateColumnIndex));
            itemChoiceManager.OnClick(view);
        }
        public void SwapCursor (ICursor newCursor)
        {
            Cursor = newCursor;
            NotifyDataSetChanged();
            EmptyView.Visibility = (ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone);
        }

        public ICursor GetCursor ()
        {
            return Cursor;
        }

        public void SelectView (RecyclerView.ViewHolder viewHolder)
        {
            if (viewHolder.GetType() == typeof(ForecastAdapterViewHolder) ) {
                var vfh = (ForecastAdapterViewHolder)viewHolder;
                OnClick(vfh,viewHolder.AdapterPosition);
            }
        }
    }

}

