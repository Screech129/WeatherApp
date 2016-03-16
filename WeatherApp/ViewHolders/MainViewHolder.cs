using System;
using Android.Views;
using Android.Widget;

namespace WeatherApp
{
	public  class MainViewHolder: Java.Lang.Object
	{
		public static ImageView iconView;
		public static TextView dateView;
		public static TextView descriptionView;
		public static TextView highTempView;
		public static TextView lowTempView;

		public MainViewHolder (View view)
		{
			iconView = (ImageView)view.FindViewById (Resource.Id.list_item_icon);
			dateView = (TextView)view.FindViewById (Resource.Id.list_item_date_textview);
			descriptionView = (TextView)view.FindViewById (Resource.Id.list_item_forecast_textview);
			highTempView = (TextView)view.FindViewById (Resource.Id.list_item_high_textview);
			lowTempView = (TextView)view.FindViewById (Resource.Id.list_item_low_textview);
		}

	}
}

