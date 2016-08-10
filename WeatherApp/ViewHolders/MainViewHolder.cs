using System;
using Android.Views;
using Android.Widget;

namespace WeatherApp
{
	public  class MainViewHolder: Java.Lang.Object
	{
		public ImageView iconView;
		public TextView dateView;
		public TextView descriptionView;
		public TextView highTempView;
		public TextView lowTempView;

		public MainViewHolder ()
		{
			
		}

	}
}

