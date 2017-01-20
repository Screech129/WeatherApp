
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
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.pref_general);
            foreach (var key in PreferenceManager.GetDefaultSharedPreferences(Activity).All.Keys)
            {

                OnSharedPreferenceChanged(PreferenceManager.GetDefaultSharedPreferences(Activity), key);
            }
        }

        public override void OnResume ()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(Activity)
            .RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause ()
        {
            base.OnPause();
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .UnregisterOnSharedPreferenceChangeListener(this);
        }


        public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(GetString(Resource.String.pref_location_key)))
            {
                // we've changed the location
                // first clear locationStatus
                Utility.ResetLocationStatus(Activity);
                SunshineSyncAdapter.SyncImmediately(Activity);
            }
            else if (key.Equals(GetString(Resource.String.pref_temp_key)))
            {
                // units have changed. update lists of weather entries accordingly
                Activity.ContentResolver.NotifyChange(WeatherContractOpen.WeatherEntryOpen.ContentUri, null);
            }
            else if (key.Equals(GetString(Resource.String.pref_location_status_key)))
            {
                // our location status has changed.  Update the summary accordingly
                var locationPreference = FindPreference(GetString(Resource.String.pref_location_key));
                BindPreferenceSummaryToValue(locationPreference);
            }
            else if (key.Equals(GetString(Resource.String.pref_art_pack_key)))
            {
                // art pack have changed. update lists of weather entries accordingly
                Activity.ContentResolver.NotifyChange(WeatherContractOpen.WeatherEntryOpen.ContentUri, null);
            }
        }

        private void SetPreferenceSummary (Preference pref, object value)
        {
            var stringValue = value.ToString();
            var key = pref.Key;

            if (pref == null)
            {
                return;
            }
            if (pref.GetType() == typeof(ListPreference))
            {
                var listPref = (ListPreference)pref;
                var prefIndex = listPref.FindIndexOfValue(stringValue);
                if (prefIndex > 0)
                    pref.Summary = listPref.Value;
            }
           
            else if (key.Equals(GetString(Resource.String.pref_location_key)))
            {
                var status = Utility.GetLocationStatus(Activity);
                switch (status)
                {
                    case (int)Helpers.LocationStatus.LocationStatusOk:
                        pref.Summary = stringValue;
                        break;
                    case (int)Helpers.LocationStatus.LocationStatusUnkown:
                        pref.Summary = string.Format(Activity.GetString(Resource.String.pref_location_unknown_description),
                            stringValue);
                        break;
                    case (int)Helpers.LocationStatus.LocationStatusInvalid:
                        pref.Summary = string.Format(Activity.GetString(Resource.String.pref_location_error_description),
                            stringValue);
                        break;
                    default:
                        pref.Summary = stringValue;
                        break;
                }
               
            }
            else
            {
                pref.Summary = stringValue;
            }
        }

        private void BindPreferenceSummaryToValue (Preference preference)
        {
            // Set the preference summaries
            SetPreferenceSummary(preference,
                    PreferenceManager
                            .GetDefaultSharedPreferences(preference.Context)
                            .GetString(preference.Key, ""));
        }

    }
}

