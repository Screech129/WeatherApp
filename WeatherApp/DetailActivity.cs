
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
using Android.Util;

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
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		
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
			if(id == Android.Resource.Id.Home){
			Finish ();
			}
			if (id == Resource.Id.action_settings) {
				var settingsIntent = new Intent(this,typeof(SettingsActivity));
				StartActivity (settingsIntent);
				return true;
			}
			if (id == Resource.Id.action_share) {
				new PlaceholderFragment ().shareWeather (item);
				return true;
			}
			return base.OnOptionsItemSelected (item);

		}

		public  class PlaceholderFragment:Fragment{
			public PlaceholderFragment ()
			{
				SetHasOptionsMenu (true);
			}
			String forecast = "";
			public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
			{
				inflater.Inflate (Resource.Menu.detail_fragment,menu);
				IMenuItem menuItem = menu.FindItem (Resource.Id.action_share);
				shareWeather (menuItem);
				base.OnCreateOptionsMenu (menu, inflater);

			}

			public override bool OnOptionsItemSelected (IMenuItem item)
			{
				int id = item.ItemId;
				if (id == Resource.Id.action_share) {
					shareWeather(item);
					return true;
				}
				return base.OnOptionsItemSelected (item);

			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				Intent intent = Activity.Intent;
				View rootView = inflater.Inflate (Resource.Layout.fragment_detail, container, false);
				if (intent != null && intent.HasExtra (Intent.ExtraText)) {
					forecast = intent.GetStringExtra (Intent.ExtraText);
					var tv = rootView.FindViewById<TextView> (Resource.Id.detail_text);
					tv.Text = forecast;
				}

				return rootView;
			}

			public void shareWeather(IMenuItem item){
				var shareText = forecast + " #SunshineApp";
				var shareIntent = new Intent (Intent.ActionSend)
					.AddFlags (ActivityFlags.ClearWhenTaskReset)
					.SetType ("text/plain")
					.PutExtra (Intent.ExtraText, shareText);
				var shareActionProvider = (ShareActionProvider)item.ActionProvider;
				if (shareActionProvider != null) {

					shareActionProvider.SetShareIntent (shareIntent);
				} else {
					Log.Debug ("DetailFragment", "Share Action Provider May Be Null");
				}

			}

		}
	}
}

