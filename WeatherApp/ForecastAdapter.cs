using System;
using Android.Widget;
using Android.Content;
using Android.Database;
using Android.Views;

namespace WeatherApp
{
	public class ForecastAdapter:CursorAdapter
	{
		public ForecastAdapter (Context context, ICursor c, CursorAdapterFlags flags) :
			base (context, c, flags)
		{
		}

		Context context = Android.App.Application.Context;



		/**
     * Prepare the weather high/lows for presentation.
     */
		private String formatHighLows (double high, double low)
		{
			bool isMetric = Utility.isMetric (context);
			String highLowStr = Utility.formatTemperature (high, isMetric) + "/" + Utility.formatTemperature (low, isMetric);
			return highLowStr;
		}

		/*
        This is ported from FetchWeatherTask --- but now we go straight from the cursor to the
        string.
     */
		private String convertCursorRowToUXFormat (ICursor cursor)
		{
			// get row indices for our cursor

			String highAndLow = formatHighLows (
				                    cursor.GetDouble (ForecastFragment.COL_WEATHER_MAX_TEMP),
				                    cursor.GetDouble (ForecastFragment.COL_WEATHER_MIN_TEMP));

			return Utility.formatDate (cursor.GetLong (ForecastFragment.COL_WEATHER_DATE)) +
			" - " + cursor.GetString (ForecastFragment.COL_WEATHER_DESC) +
			" - " + highAndLow;
		}

		/*
        Remember that these views are reused as needed.
     */
		public override View NewView (Context context, ICursor cursor, ViewGroup parent)
		{
			View view = LayoutInflater.From (context).Inflate (Resource.Layout.list_item_forecast, parent, false);

			return view;
		}
		/*
        This is where we fill-in the views with the contents of the cursor.
     */
		public override void BindView (View view, Context context, ICursor cursor)
		{
			// our view is pretty simple here --- just a text view
			// we'll keep the UI functional with a simple (and slow!) binding.
			TextView tv = (TextView)view;
			tv.Text = convertCursorRowToUXFormat (cursor);
		}
			
	}

}

