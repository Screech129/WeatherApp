
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Preferences;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.App;
using WeatherApp.Sync;

namespace WeatherApp
{
	public class SettingsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
	{
		
		public SettingsFragment ()
		{

		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			AddPreferencesFromResource (Resource.Xml.pref_general);
			foreach (var key in PreferenceManager.GetDefaultSharedPreferences (Activity).All.Keys) {
				
				OnSharedPreferenceChanged (PreferenceManager.GetDefaultSharedPreferences (Activity), key);
			}
		}

		public override void OnResume ()
		{
			base.OnResume ();
			PreferenceManager.GetDefaultSharedPreferences (Activity)
			.RegisterOnSharedPreferenceChangeListener (this);
		}

		public override void OnPause ()
		{
			base.OnPause ();
			PreferenceManager.GetDefaultSharedPreferences (Activity)
				.RegisterOnSharedPreferenceChangeListener (this);
		}

		public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
		{
			Preference pref = FindPreference (key);
            var prefEditor = sharedPreferences.Edit();
            if (pref == null)
            {
                return;
            }
            if (pref.GetType () == typeof(ListPreference)) {
				var listPref = (ListPreference)pref;
				pref.Summary = listPref.Entry;
			}
            else if (pref.GetType() == typeof(CheckBoxPreference))
            {
                pref.Summary = sharedPreferences.GetBoolean(key,true).ToString();
                prefEditor.PutBoolean(key, sharedPreferences.GetBoolean(key, true));

            }
            else {
				pref.Summary = sharedPreferences.GetString (key, "");
                prefEditor.PutString(key, sharedPreferences.GetString(key, ""));
            }
			
			prefEditor.Commit ();
            SunshineSyncAdapter.SyncImmediately(Activity);
		}


	}
}

