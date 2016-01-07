
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WeatherApp
{
	[Activity (Label = "DetailActivity")]			
	public class DetailActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (WindowFeatures.ActionBar);
			SetContentView (Resource.Layout.activity_detail);

		
			if (savedInstanceState == null) {
				FragmentTransaction fragTx = this.FragmentManager.BeginTransaction ();

				fragTx.Add (Resource.Id.container, new PlaceholderFragment ())
					.Commit ();
			   


			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.detail,menu);
			return base.OnCreateOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_settings) {
				var settingsIntent = new Intent(this,typeof(SettingsActivity));
				StartActivity (settingsIntent);
				return true;
			}

			return base.OnOptionsItemSelected (item);

		}

		public  class PlaceholderFragment:Fragment{
			public PlaceholderFragment ()
			{
				
			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				Intent intent = Activity.Intent;
				View rootView = inflater.Inflate (Resource.Layout.fragment_detail, container, false);
				if (intent != null && intent.HasExtra (Intent.ExtraText)) {
					String forecast = intent.GetStringExtra (Intent.ExtraText);
					var tv = rootView.FindViewById<TextView> (Resource.Id.detail_text);
					tv.Text = forecast;
				}

				return rootView;
			}

		}
	}
}

